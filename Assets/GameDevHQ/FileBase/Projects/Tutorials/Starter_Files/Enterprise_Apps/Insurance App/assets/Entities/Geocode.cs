using Newtonsoft.Json;
using System.Collections.Generic;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AddressComponent
{
    [JsonProperty("long_name")]
    public string LongName;

    [JsonProperty("short_name")]
    public string ShortName;

    [JsonProperty("types")]
    public List<string> Types;
}

public class Geometry
{
    [JsonProperty("location")]
    public Location Location;

    [JsonProperty("location_type")]
    public string LocationType;

    [JsonProperty("viewport")]
    public Viewport Viewport;
}

public class Location
{
    [JsonProperty("lat")]
    public double Lat;

    [JsonProperty("lng")]
    public double Lng;
}

public class Northeast
{
    [JsonProperty("lat")]
    public double Lat;

    [JsonProperty("lng")]
    public double Lng;
}

public class PlusCode
{
    [JsonProperty("compound_code")]
    public string CompoundCode;

    [JsonProperty("global_code")]
    public string GlobalCode;
}

public class Result
{
    [JsonProperty("address_components")]
    public List<AddressComponent> AddressComponents;

    [JsonProperty("formatted_address")]
    public string FormattedAddress;

    [JsonProperty("geometry")]
    public Geometry Geometry;

    [JsonProperty("place_id")]
    public string PlaceId;

    [JsonProperty("plus_code")]
    public PlusCode PlusCode;

    [JsonProperty("types")]
    public List<string> Types;
}

public class Southwest
{
    [JsonProperty("lat")]
    public double Lat;

    [JsonProperty("lng")]
    public double Lng;
}

public class Viewport
{
    [JsonProperty("northeast")]
    public Northeast Northeast;

    [JsonProperty("southwest")]
    public Southwest Southwest;
}



public class Geocode 
{
    [JsonProperty("plus_code")]
    public PlusCode PlusCode;

    [JsonProperty("results")]
    public List<Result> Results;

    [JsonProperty("status")]
    public string Status;
}
