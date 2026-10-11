using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Class)]
public class InlineFieldAttribute : PropertyAttribute
{
    public string Template;

    public InlineFieldAttribute(string template = null)
    {
        Template = template;
    }
}
