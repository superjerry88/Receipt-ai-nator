namespace WebApp.DB.Core;

[AttributeUsage(AttributeTargets.Class)]
public class CollectionName(string str) : Attribute
{
    public string Value { get; set; } = str;
}