using System.Collections;
using System;
using System.Web;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

[System.Serializable]
public class MapsAPIHandler
{
    public string googleStaticMapsAPIkey = "AIzaSyBROF0pHek1BdEGzgRE1RKxWqg84iwl1LI";
    public string staticMapUrl = "https://maps.googleapis.com/maps/api/staticmap";
    public string geocodeUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    public string mapPath = "";

    private float _waitTimeSeconds = 20f;
    private bool _hasCoordinates = false;
    private bool _isError;

    public MapsRequest mapsRequest = new MapsRequest();
    private CultureInfo _ci = new CultureInfo("en-US");

    public Dictionary<API, string> jsonResponses = new Dictionary<API, string>();


    #region API request
    public void GetStaticMapRequest(int width, int height)
    {
        mapsRequest.XSize = width;
        mapsRequest.YSize = height;

        var uriBuilder = new UriBuilder(staticMapUrl);
        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);

        paramValues.Add("key", googleStaticMapsAPIkey);
        paramValues.Add("center", mapsRequest.Latitude.ToString(_ci) + "," + mapsRequest.Longitude.ToString(_ci));
        paramValues.Add("markers", mapsRequest.Latitude.ToString(_ci) + "," + mapsRequest.Longitude.ToString(_ci));
        paramValues.Add("zoom", mapsRequest.Zoom.ToString());
        paramValues.Add("size", mapsRequest.XSize.ToString() + "x" + mapsRequest.YSize.ToString());

        uriBuilder.Query = paramValues.ToString();
        mapsRequest.Url = uriBuilder.ToString();
    }

    public void GetLocationRequest()
    {
        var uriBuilder = new UriBuilder(geocodeUrl);
        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);

        paramValues.Add("key", googleStaticMapsAPIkey);
        paramValues.Add("latlng", mapsRequest.Latitude.ToString(_ci) + "," + mapsRequest.Longitude.ToString(_ci));
        paramValues.Add("location_type", "ROOFTOP");
        paramValues.Add("result_type", "street_address");

        uriBuilder.Query = paramValues.ToString();
        mapsRequest.Url = uriBuilder.ToString();
    }
    #endregion

    #region Map
    public IEnumerator DownloadMap(RawImage image, int width, int height, Action<bool> errorCallback)
    {
        float startTime = Time.time;
        yield return new WaitUntil(() => _hasCoordinates || Time.time - startTime == _waitTimeSeconds);

        if (_hasCoordinates)
        {
            GetStaticMapRequest(width, height);
            UnityWebRequest web = UnityWebRequestTexture.GetTexture(mapsRequest.Url);

            yield return web.SendWebRequest();


            if (web.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(web.error);
                _isError = true;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(web);
                SaveMapToPNG(texture.EncodeToPNG());
                image.texture = texture;
            }

            web.Dispose();
        }
        else
        {
            _isError = true;
            Debug.LogWarning("DownloadMap() timed out!");
        }

        errorCallback(_isError);
    }

    private void SaveMapToPNG(byte[] bytes)
    {
        string caseNum =
                (InsuranceAppUIManager.Instance.activeCase != null
                ? InsuranceAppUIManager.Instance.activeCase.CaseNumber
                : "NO_CASE");

        string subFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + caseNum;

        if (!Directory.Exists(subFolderPath))
            Directory.CreateDirectory(subFolderPath);

        mapPath = subFolderPath + Path.DirectorySeparatorChar + caseNum + "_map.png";
        File.WriteAllBytes(mapPath, bytes);
    }
    #endregion

    #region Address
    public IEnumerator GetLocation(Action<bool> errorCallback)
    {
        float startTime = Time.time;
        yield return new WaitUntil(() => _hasCoordinates || Time.time - startTime == _waitTimeSeconds);

        if (_hasCoordinates)
        {
            GetLocationRequest();
            UnityWebRequest web = UnityWebRequest.Get(mapsRequest.Url);
            web.method = "GET";

            yield return web.SendWebRequest();

            if (web.result == UnityWebRequest.Result.ConnectionError || web.responseCode != 200)
            {
                Debug.LogError(web.error);
                _isError = true;
            }
            else
                SecureAddJson(API.geocode, web.downloadHandler.text);

            web.Dispose();
        }
        else
        {
            _isError = true;
            Debug.LogWarning("GetLocation() timed out!");
        }

        errorCallback(_isError);
    }

    public IEnumerator GetLocation(TextMeshProUGUI textMesh, Action<bool> errorCallback)
    {
        float startTime = Time.time;
        yield return new WaitUntil(() => _hasCoordinates || Time.time - startTime == _waitTimeSeconds);
        bool hasResponse = false;

        if (_hasCoordinates)
        {
            GetLocationRequest();
            UnityWebRequest web = UnityWebRequest.Get(mapsRequest.Url);
            web.method = "GET";

            yield return web.SendWebRequest();

            if (web.result == UnityWebRequest.Result.ConnectionError || web.responseCode != 200)
            {
                _isError = true;
                Debug.LogError(web.error);
                textMesh.text = "Unable to retrieve address information!";
            }
            else
                hasResponse = SecureAddJson(API.geocode, web.downloadHandler.text);

            web.Dispose();
        }
        else
        {
            _isError = true;
            Debug.LogWarning("GetLocation() timed out!");
        }

        if (hasResponse)
            textMesh.text = GetAddress(jsonResponses[API.geocode]);

        errorCallback(_isError);
    }

    public string GetAddress(string json)
    {
        Geocode geocode = JsonConvert.DeserializeObject<Geocode>(json);
        return geocode != null && geocode.Results.Count > 0
            ? geocode.Results[0].FormattedAddress
            : "Unable to retrieve address information!";
    }

    private bool SecureAddJson(API api, string json)
    {
        if (jsonResponses.ContainsKey(api))
            jsonResponses.Remove(api);

        return jsonResponses.TryAdd(api, json);
    }

    #endregion

    #region GPS
    public IEnumerator GetGPSCoordinates(Action<bool> errorCallback)
    {
#if UNITY_EDITOR
        _hasCoordinates = true;
        mapsRequest.Latitude = 40.85481f;
        mapsRequest.Longitude = 14.27714f;
#endif
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location Services are disabled.");
            yield break;
        }

        Input.location.Start();
        float startTime = Time.time;
        yield return new WaitUntil(() => Input.location.status == LocationServiceStatus.Running || Time.time - startTime == _waitTimeSeconds);


        switch (Input.location.status)
        {
            case LocationServiceStatus.Initializing:
                {
                    _isError = true;
                    Debug.LogWarning("GetGPSCoordinates() timed out!");
                    yield break;
                }
            case LocationServiceStatus.Failed:
            case LocationServiceStatus.Stopped:
                {
                    _isError = true;
                    Debug.LogWarning("GetGPSCoordinates() failed to retreive coordinates!");
                    yield break;
                }
            default:
                {
                    mapsRequest.Latitude = Input.location.lastData.latitude;
                    mapsRequest.Longitude = Input.location.lastData.longitude;
                    _hasCoordinates = true;

                    Input.location.Stop();
                    break;
                }
        }

        errorCallback(_isError);
    }
    #endregion

}
public enum API
{
    geocode,
    staticmap
}