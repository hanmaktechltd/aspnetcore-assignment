using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class AtLeastOneElementAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is ICollection collection)
        {
            return collection.Count > 0;
        }

        return false;
    }
}
