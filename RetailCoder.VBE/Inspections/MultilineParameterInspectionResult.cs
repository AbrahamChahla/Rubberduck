﻿using System.Collections.Generic;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Symbols;
using Rubberduck.UI;

namespace Rubberduck.Inspections
{
    public class MultilineParameterInspectionResult : InspectionResultBase
    {
        private readonly IEnumerable<CodeInspectionQuickFix> _quickFixes;

        public MultilineParameterInspectionResult(IInspection inspection, Declaration target)
            : base(inspection, target)
        {
            _quickFixes = new CodeInspectionQuickFix[]
            {
                new MakeSingleLineParameterQuickFix(Context, QualifiedSelection),
                new IgnoreOnceQuickFix(Target.ParentDeclaration.Context, Target.ParentDeclaration.QualifiedSelection, Inspection.AnnotationName) 
            };
        }

        public override string Description
        {
            get
            {
                return string.Format(
                    Target.Context.GetSelection().LineCount > 3
                        ? RubberduckUI.EasterEgg_Continuator
                        : InspectionsUI.MultilineParameterInspectionResultFormat, Target.IdentifierName);
            }
        }

        public override IEnumerable<CodeInspectionQuickFix> QuickFixes { get { return _quickFixes; } }
    }
}
