namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Allows a request type (e.g Command, Query, Request, Event) to have its handler resolved polymorphically. 
/// This means that if a request type is derived from a base request type, 
/// the handler for the base request type can be used to handle the derived request type.
/// </summary>
/// <remarks>
/// This option may be useful in specifying only specific classes in cases where you don't want to enable polymorphic resolution for all classes.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PolymorphicResolutionAttribute : Attribute
{

}
