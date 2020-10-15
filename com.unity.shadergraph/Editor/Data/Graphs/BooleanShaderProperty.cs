using System;
using System.Text;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.ShaderGraph.Internal
{
    [Serializable]
    [FormerName("UnityEditor.ShaderGraph.BooleanShaderProperty")]
    [BlackboardInputInfo(20)]
    public sealed class BooleanShaderProperty : AbstractShaderProperty<bool>
    {
        internal PropertyHLSLGenerationType m_generationType = PropertyHLSLGenerationType.UnityPerMaterial;

        internal BooleanShaderProperty()
        {
            displayName = "Boolean";
        }

        public override PropertyType propertyType => PropertyType.Boolean;

        internal override bool isExposable => true;
        internal override bool isRenamable => true;

        internal override string GetPropertyAsArgumentString()
        {
            return $"{concreteShaderValueType.ToShaderString(concretePrecision.ToShaderString())} {referenceName}";
        }

        internal override void AppendPropertyDeclarations(ShaderStringBuilder builder, Func<string, string> nameModifier, PropertyHLSLGenerationType generationTypes)
        {
            if ((generationTypes & m_generationType) != 0)
            {
                string name = nameModifier?.Invoke(referenceName) ?? referenceName;
                builder.AppendLine($"{concreteShaderValueType.ToShaderString(concretePrecision.ToShaderString())} {name};");
            }
        }

        internal override string GetPropertyBlockString()
        {
            return $"{hideTagString}[ToggleUI]{referenceName}(\"{displayName}\", Float) = {(value == true ? 1 : 0)}";
        }

        internal override AbstractMaterialNode ToConcreteNode()
        {
            return new BooleanNode { value = new ToggleData(value) };
        }

        internal override PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty(propertyType)
            {
                name = referenceName,
                booleanValue = value
            };
        }

        internal override ShaderInput Copy()
        {
            return new BooleanShaderProperty()
            {
                displayName = displayName,
                hidden = hidden,
                value = value,
                precision = precision,
            };
        }
    }
}
