using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

namespace LsAdmin.Application.EquipmentApp.Dtos {
    public class ActiveEquipmentDto {
        [Key]
        public Guid EquipmentId {
            get {
                return Equipment?.Id == null ? Guid.Empty : Equipment.Id;
            }
        }
        public string DeviceId { get; set; }
        public EquipmentDto Equipment { get; set; }
        public string Token { get; set; }
        public DateTime OnlineTime { get; set; }
        public DateTime LastConnectTime { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public WebSocket WebSocket { get; set; }
        public byte[] LastScreenshot { get; set; }
        public DateTime LastScreenshotTime { get; set; }
        public EquipmentInfoDto EquipmentInfo { get; set; }
        public PlayInfoDto PlayInfo { get; set; }
        public EquipmentNetworkInfoDto NetworkInfo { get; set; }
        public RealtimeInfoDto RealtimeInfo { get; set; }
        public AppVersionDto AppVersion { get; set; }

        public class PlayInfoDto {
            public string PlayInfo;
            public string PlayerPlayInfo;
            public string Player;
        }
        public class EquipmentInfoDto {

            public string SystemVersion { get; set; }
            public string SystemModel { get; set; }
            public string DeviceBrand { get; set; }
            public string CpuName { get; set; }
            public string TotalRam { get; set; }
            public string SdCardTotalMemory { get; set; }
            public string MacAddress { get; set; }
            public string ScreenResolutionWidth { get; set; }
            public string ScreenResolutionHeight { get; set; }
            public string ScreenOrientation { get; set; }
            public string ScreenBrightnessMode { get; set; }
            public string ScreenBrightness { get; set; }
            public string Volume { get; set; }
            public string SystemMaxVolume { get; set; }
        }
        public class EquipmentNetworkInfoDto {

            public bool IsNetworkConnected { get; set; }
            public bool IsWifiConnected { get; set; }
            public bool IsMobileConnected { get; set; }
            public string ConnectedType { get; set; }
            public string WifiInfo { get; set; }
            public string WifiList { get; set; }
        }
        public class RealtimeInfoDto {

            public string SdCardAvailableMemory { get; set; }
            public string AvailableRam { get; set; }
        }
        public class AppVersionDto {

            public string VersionName { get; set; }
            public string PackageName { get; set; }
        }
    }
}