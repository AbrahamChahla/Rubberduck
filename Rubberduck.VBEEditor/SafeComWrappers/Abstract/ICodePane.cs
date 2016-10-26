using System;

namespace Rubberduck.VBEditor.SafeComWrappers.Abstract
{
    public interface ICodePane : ISafeComWrapper, IEquatable<ICodePane>
    {
        IVBE VBE { get; }
        ICodePanes Collection { get; }
        IWindow Window { get; }
        int TopLine { get; set; }
        int CountOfVisibleLines { get; }
        ICodeModule CodeModule { get; }
        CodePaneView CodePaneView { get; }
        Selection GetSelection();
        QualifiedSelection? GetQualifiedSelection();
        void SetSelection(int startLine, int startColumn, int endLine, int endColumn);
        void SetSelection(Selection selection);
        void Show();
    }
}