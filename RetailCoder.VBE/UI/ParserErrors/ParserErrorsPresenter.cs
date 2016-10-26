﻿using System.ComponentModel;
using System.Windows.Forms;
using Rubberduck.Parsing.VBA;
using Rubberduck.UI.IdentifierReferences;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.UI.ParserErrors
{
    public interface IParserErrorsPresenter
    {
        void Show();
        void AddError(ParseErrorEventArgs error);
        void Clear();
    }

    public class ParserErrorsPresenter : DockableToolwindowPresenter, IParserErrorsPresenter
    {
        public ParserErrorsPresenter(IVBE vbe, IAddIn addin) 
            : base(vbe, addin, new SimpleListControl(RubberduckUI.ParseErrors_Caption))
        {
            _source = new BindingList<ParseErrorListItem>();
            Control.Navigate += Control_Navigate;
        }

        void Control_Navigate(object sender, ListItemActionEventArgs e)
        {
            var selection = (ParseErrorListItem) e.SelectedItem;
            selection.Navigate();
        }

        private SimpleListControl Control { get { return (SimpleListControl) UserControl; } }

        private readonly IBindingList _source;

        public void AddError(ParseErrorEventArgs error)
        {
            _source.Add(new ParseErrorListItem(error));
            var control = Control;
            if (control.InvokeRequired)
            {
                control.Invoke((MethodInvoker) delegate
                {
                    Control.ResultBox.DataSource = _source;
                    Control.ResultBox.DisplayMember = "Value";
                    control.Refresh();
                });
            }
            else
            {
                Control.ResultBox.DataSource = _source;
                Control.ResultBox.DisplayMember = "Value";
                control.Refresh();
            }
        }

        public void Clear()
        {
            _source.Clear();
        }
    }
}
