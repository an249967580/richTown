using System;
using UnityEngine;

namespace RT
{
    // 地理位置
    public class Location : MonoBehaviour
    {
        public static Location Instance;

        private double _latitude, _longitude;
        private bool _isLocated;
        private const double EARTH_RADIUS = 6378.137;// 单位千米  

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartGps();
        }

        public void StopGps()
        {
            Input.location.Stop();
        }

        public void StartGps()
        {
            Input.location.Start();
            InvokeRepeating("located", 2.0f, 1.0f);
        }

        void located()
        {
            if(!_isLocated)
            {
                if(Input.location.status != LocationServiceStatus.Initializing)
                {
                    if (Input.location.status == LocationServiceStatus.Failed)
                    {
                        Debug.Log("located failed");
                    }
                    else if (Input.location.status == LocationServiceStatus.Stopped)
                    {
                        Debug.Log("located stopped");
                    }
                    else if (Input.location.status == LocationServiceStatus.Running)
                    {
                        _isLocated = true;
                        _latitude = Input.location.lastData.latitude;
                        _longitude = Input.location.lastData.longitude;
                        InvokeRepeating("saveLocation", 2, 3);
                        StopGps();
                    }
                    CancelInvoke("located");
                }
            }
        }

        private void saveLocation()
        {
            if (Game.Instance.CurPlayer != null && Validate.IsNotEmpty(Game.Instance.CurPlayer.SessionId))
            {
                if(_isLocated)
                {
                    UserApi.SaveLocation(_latitude, _longitude, (rsp) =>
                    {
                        if(rsp.IsOk)
                        {
                            CancelInvoke("saveLocation");
                        }
                        else
                        {
                            Game.Instance.ShowTips(rsp.errorMsg);
                        }
                    });
                }
            }
        }

        public double Distance(double lat, double lng)
        {
            double radLat1 = getRadian(_latitude);
            double radLat2 = getRadian(lat);
            double a = radLat1 - radLat2;
            double b = getRadian(_longitude) - getRadian(lng);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1)
                * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            return s * 1000;
        }
        public double Distance(double xlat, double xlng, double ylat, double ylng)
        {
            double radLat1 = getRadian(xlat);
            double radLat2 = getRadian(ylat);
            double a = radLat1 - radLat2;
            double b = getRadian(xlng) - getRadian(ylng);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1)
                * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            return s * 1000;
        }
        private double getRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }

    }
}
