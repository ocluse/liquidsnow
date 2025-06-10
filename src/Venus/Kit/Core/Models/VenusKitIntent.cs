using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Models;
[JsonDerivedType(typeof(NavigateVenusKitIntent), "Navigate")]
public abstract record VenusKitIntent
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static VenusKitIntent FromJson(string json)
    {
        return JsonSerializer.Deserialize<VenusKitIntent>(json)
            ?? throw new InvalidOperationException("Failed to deserialize VenusKitIntent");
    }
}

public record NavigateVenusKitIntent : VenusKitIntent
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly Dictionary<DestinationType, Type> _dataTypes = new()
    {
        { DestinationType.LivePage, typeof(RoomInitializationOptions) },
        {DestinationType.UserReadPage, typeof(UserReadWithIdOptions) },
        {DestinationType.ConversationReadPage, typeof(ConversationReadOptions) }
    };

    private static readonly Dictionary<DestinationType, Type> _pageTypes = new()
    {
        { DestinationType.LivePage, typeof(LivePage) },
        { DestinationType.UserReadPage, typeof(UserReadPage) },
        { DestinationType.ConversationReadPage, typeof(ConversationReadPage) }
    };

    [JsonConstructor]
    private NavigateVenusKitIntent(DestinationType destination, string? serializedData)
    {
        Destination = destination;
        SerializedData = serializedData;
    }

    public string? SerializedData { get; }

    public DestinationType Destination { get; }

    [JsonIgnore]
    public object? Data
    {
        get
        {
            if (SerializedData is null)
            {
                return null;
            }
            else
            {
                if (_dataTypes.TryGetValue(Destination, out var type))
                {
                    return JsonSerializer.Deserialize(SerializedData, type, _options);
                }
                else
                {
                    throw new ArgumentException($"No data type found for destination {Destination}");
                }
            }
        }
    }

    [JsonIgnore]
    public Type Page
    {
        get
        {
            if (_pageTypes.TryGetValue(Destination, out var type))
            {
                return type;
            }
            else
            {
                throw new ArgumentException($"No page type found for destination {Destination}");
            }
        }
    }

    public static NavigateVenusKitIntent Create(DestinationType destination, object data)
    {
        if (_dataTypes.TryGetValue(destination, out var type))
        {
            var serializedData = JsonSerializer.Serialize(data, type, _options);
            return new NavigateVenusKitIntent(destination, serializedData);
        }
        else
        {
            throw new ArgumentException($"No data type found for destination {destination}");
        }
    }

    public static NavigateVenusKitIntent Create(DestinationType destination)
    {
        return new NavigateVenusKitIntent(destination, null);
    }
}