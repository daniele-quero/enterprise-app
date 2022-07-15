using System;
using System.Collections.Generic;

[Serializable]
class Geocode2
{

    public List<Result2> results;

}
[Serializable]
public class Result2
{
    public string formatted_address;
}