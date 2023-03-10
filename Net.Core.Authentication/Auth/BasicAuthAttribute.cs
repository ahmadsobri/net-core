using Microsoft.AspNetCore.Mvc;
using Net.Core.Authentication.Auth;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthAttribute : TypeFilterAttribute
{
    public BasicAuthAttribute(string realm = @"My Realm") : base(typeof(BasicAuthFilter))
    {
        Arguments = new object[] { realm };
    }
}