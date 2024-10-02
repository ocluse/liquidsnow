using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Http;

/// <summary>
/// Represents a URL.
/// </summary>
public class Url
{
    private class UrlPart(string value, bool isParameter)
    {
        public string Value { get; } = value;

        public bool IsParameter { get; } = isParameter;
    }

    private enum UrlParameterType
    {
        Path, 
        Query
    }

    private record UrlParameter(UrlParameterType Type, string Key, string Value);

    private readonly List<UrlPart> _parts = [];

    private readonly List<UrlParameter> _parameters = [];

    public string? Base { get; set; }

    public static Url Create(string path)
    {
        string[] components = path.Split("://");
        
        string? protocol = components.Length > 1 ? components[0] : null;
        
        string[] parts = path.Split('/');
    }
}
