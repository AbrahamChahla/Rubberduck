﻿using System.IO;
using Infralution.Localization.Wpf;
using NLog;
using Rubberduck.Common;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Settings;
using Rubberduck.UI;
using Rubberduck.UI.Command.MenuItems;
using System;
using System.Globalization;
using System.Windows.Forms;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck
{
    public sealed class App : IDisposable
    {
        private readonly IVBE _vbe;
        private readonly IMessageBox _messageBox;
        private readonly IRubberduckParser _parser;
        private readonly AutoSave.AutoSave _autoSave;
        private readonly IGeneralConfigService _configService;
        private readonly IAppMenu _appMenus;
        private readonly RubberduckCommandBar _stateBar;
        private readonly IRubberduckHooks _hooks;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private Configuration _config;

        public App(IVBE vbe, 
            IMessageBox messageBox,
            IRubberduckParser parser,
            IGeneralConfigService configService,
            IAppMenu appMenus,
            RubberduckCommandBar stateBar,
            IRubberduckHooks hooks)
        {
            _vbe = vbe;
            _messageBox = messageBox;
            _parser = parser;
            _configService = configService;
            _autoSave = new AutoSave.AutoSave(_vbe, _configService);
            _appMenus = appMenus;
            _stateBar = stateBar;
            _hooks = hooks;

            _hooks.MessageReceived += _hooks_MessageReceived;
            _configService.SettingsChanged += _configService_SettingsChanged;
            _parser.State.StateChanged += Parser_StateChanged;
            _parser.State.StatusMessageUpdate += State_StatusMessageUpdate;
            _stateBar.Refresh += _stateBar_Refresh;

            UiDispatcher.Initialize();
        }

        private void State_StatusMessageUpdate(object sender, RubberduckStatusMessageEventArgs e)
        {
            var message = e.Message;
            if (message == ParserState.LoadingReference.ToString())
            {
                // note: ugly hack to enable Rubberduck.Parsing assembly to do this
                message = RubberduckUI.ParserState_LoadingReference;
            }

            _stateBar.SetStatusText(message);
        }

        private void _hooks_MessageReceived(object sender, HookEventArgs e)
        {
            RefreshSelection();
        }

        private ParserState _lastStatus;
        private Declaration _lastSelectedDeclaration;

        private void RefreshSelection()
        {
            var pane = _vbe.ActiveCodePane;
            {
                Declaration selectedDeclaration = null;
                if (!pane.IsWrappingNullReference)
                {
                    selectedDeclaration = _parser.State.FindSelectedDeclaration(pane);
                    _stateBar.SetSelectionText(selectedDeclaration);
                }

                var currentStatus = _parser.State.Status;
                if (ShouldEvaluateCanExecute(selectedDeclaration, currentStatus))
                {
                    _appMenus.EvaluateCanExecute(_parser.State);
                }

                _lastStatus = currentStatus;
                _lastSelectedDeclaration = selectedDeclaration;
            }
        }

        private bool ShouldEvaluateCanExecute(Declaration selectedDeclaration, ParserState currentStatus)
        {
            return _lastStatus != currentStatus ||
                   (selectedDeclaration != null && !selectedDeclaration.Equals(_lastSelectedDeclaration)) ||
                   (selectedDeclaration == null && _lastSelectedDeclaration != null);
        }

        private void _configService_SettingsChanged(object sender, ConfigurationChangedEventArgs e)
        {
            _config = _configService.LoadConfiguration();
            _hooks.HookHotkeys();
            // also updates the ShortcutKey text
            _appMenus.Localize();
            UpdateLoggingLevel();

            if (e.LanguageChanged)
            {
                LoadConfig();
            }
        }

        private void EnsureDirectoriesExist()
        {
            try
            {
                if (!Directory.Exists(ApplicationConstants.LOG_FOLDER_PATH))
                {
                    Directory.CreateDirectory(ApplicationConstants.LOG_FOLDER_PATH);
                }
            }
            catch
            {
                //Does this need to display some sort of dialog?
            }
        }

        private void UpdateLoggingLevel()
        {
            LogLevelHelper.SetMinimumLogLevel(LogLevel.FromOrdinal(_config.UserSettings.GeneralSettings.MinimumLogLevel));
        }

        public void Startup()
        {
            EnsureDirectoriesExist();
            LoadConfig();
            _appMenus.Initialize();
            _stateBar.Initialize();
            _hooks.HookHotkeys(); // need to hook hotkeys before we localize menus, to correctly display ShortcutTexts
            _appMenus.Localize();
            _stateBar.SetStatusText(RubberduckUI.ParserState_Pending);
            UpdateLoggingLevel();
        }

        public void Shutdown()
        {
            try
            {
                _hooks.Detach();
            }
            catch
            {
                // Won't matter anymore since we're shutting everything down anyway.
            }
        }

        private void _stateBar_Refresh(object sender, EventArgs e)
        {
            // handles "refresh" button click on "Rubberduck" command bar
            _parser.State.OnParseRequested(sender);
            _parser.State.StartEventSinks(); // no-op if already started
        }

        private void Parser_StateChanged(object sender, EventArgs e)
        {
            Logger.Debug("App handles StateChanged ({0}), evaluating menu states...", _parser.State.Status);
            _appMenus.EvaluateCanExecute(_parser.State);
            _stateBar.SetSelectionText();
        }

        private void LoadConfig()
        {
            _config = _configService.LoadConfiguration();
            _autoSave.ConfigServiceSettingsChanged(this, EventArgs.Empty);

            var currentCulture = RubberduckUI.Culture;
            try
            {
                CultureManager.UICulture = CultureInfo.GetCultureInfo(_config.UserSettings.GeneralSettings.Language.Code);
                _appMenus.Localize();
            }
            catch (CultureNotFoundException exception)
            {
                Logger.Error(exception, "Error Setting Culture for Rubberduck");
                _messageBox.Show(exception.Message, "Rubberduck", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _config.UserSettings.GeneralSettings.Language.Code = currentCulture.Name;
                _configService.SaveConfiguration(_config);
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_parser != null && _parser.State != null)
            {
                _parser.State.StateChanged -= Parser_StateChanged;
                _parser.State.StatusMessageUpdate -= State_StatusMessageUpdate;
            }

            if (_hooks != null)
            {
                _hooks.MessageReceived -= _hooks_MessageReceived;
            }

            if (_configService != null)
            {
                _configService.SettingsChanged -= _configService_SettingsChanged;
            }

            if (_stateBar != null)
            {
                _stateBar.Refresh -= _stateBar_Refresh;
            }

            if (_autoSave != null)
            {
                _autoSave.Dispose();
            }

            UiDispatcher.Shutdown();

            _disposed = true;
        }
    }
}
