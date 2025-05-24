﻿using System;
using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Variables;
using Emilia.Variables.Editor;

namespace Emilia.Node.Universal.Editor
{
    [SelectedClear]
    public class UniversalEditorParametersManage : EditorParametersManage
    {
        public override IList<Type> filterTypes => new List<Type>() {
            typeof(VariableSingle),
            typeof(VariableInt32),
            typeof(VariableString),
            typeof(VariableVector2),
            typeof(VariableVector3),
            typeof(VariableObject),
        };
    }
}