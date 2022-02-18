using ActiveEquipment.Application.DataModel;
using EquipmentService.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static LsAdmin.Application.EquipmentApp.Dtos.ActiveEquipmentDto;

namespace EquipmentService.WebAPI.Controllers {
    interface IInstructionController
    {
        /// <summary>
        /// 默认指令执行结果通知接收器
        /// </summary>
        /// <param name="dto">指令执行结果</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult InstructionDefaultNotify(InstructionResultDto dto);

        /// <summary>
        /// 接收设备原始指令的接收结果通知（设备在接收到之后会发送这个通知到服务器提醒已收到指令）
        /// </summary>
        /// <param name="dto">原始指令的接收结果通知</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult InstructionOriginalNotify(OriginalInstructionNotifyDto dto);

        /// <summary>
        /// 上传APP运行错误
        /// </summary>
        /// <param name="log">错误日志</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult SystemErrorNotify(string log, string token, long timestamp, string sign);

        /// <summary>
        /// 同步节目列表
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        ///   {
        ///         code=1,
        ///         playInfo=[
        ///            {playInfoId="id1", launcher="index.html", duration=50000, fileMd5="abcdefgmd5, type=1"},
        ///            {playInfoId="id2", launcher="index.html", duration=50000, fileMd5="hijklmnmd5, type=1"},
        ///            {playInfoId="id3", launcher="video.mp4", duration=50000, fileMd5="abcdefgmd5, type=2"},
        ///            {playInfoId="id4", launcher="video.mp4", duration=50000, fileMd5="hijklmnmd5, type=2"}],
        ///         playerPlayInfo =[
        ///            {playInfoId="id1", playerId="player1",sort=1},
        ///            {playInfoId="id2", playerId="player1",sort=2}
        ///            {playInfoId="id3", playerId="player2",sort=1},
        ///            {playInfoId="id4", playerId="player2",sort=2}]
        ///   }
        /// </returns>
        /// 
        JsonResult SyncPlayInfoList(string token, long timestamp, string sign);

        /// <summary>
        /// 同步播放器列表
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        ///  {
        ///  code=1,
        ///  dataList=[
        ///       {playerId="player1", width=1024, height=100, x=0, y=0, sort=1 },
        ///       {playerId="player2", width=1024, height=720, x=0, y=100, sort=2 }]
        ///  }
        /// </returns>
        /// 
        JsonResult SyncPlayerList(string token, long timestamp, string sign);

        /// <summary>
        /// 同步节目资源列表
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        ///  {
        ///     code=1,
        ///     dataList=[
        ///         {playInfoId="id1", fileMd5="abcdefgmd5"},
        ///         {playInfoId="id2", fileMd5="hijklmnmd5"}]
        ///  }
        /// </returns>
        /// 
        JsonResult SyncPlayInfoResourcesList(string token, long timestamp, string sign);

        /// <summary>
        /// 同步操作字典
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        ///  {
        ///     code=1,
        ///     version="1.0"
        ///     dataList=[
        ///         {key="key1", type=1, method="test1"},
        ///         {key="key2", type=1, method="test2"}]
        ///  }                                                 
        /// </returns>
        JsonResult SyncOperationDictionary(string token, long timestamp, string sign);

        /// <summary>
        /// 同步定时器列表
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        ///  {
        ///     code=1,
        ///     dataList=[
        ///         {alarmId="alarmId1", sign="abcd1234", time=123456789, dateSetting="{single:{date:\"2018-04-27,2018-04-28\"},repeat:{dayInWeek:\"1,3,5\",dayInMonth:\"1,10,15,30\",weekInMoth:\"1,3\",monthInYear:\"1,2,7,8\"}}", notifyUrl="/", key="test"},
        ///         {alarmId="alarmId2", sign="efgh56789", time=123456789, dateSetting="{single:{date:\"2018-04-27,2018-04-28\"},repeat:{dayInWeek:\"1,3,5\",dayInMonth:\"1,10,15,30\",weekInMoth:\"1,3\",monthInYear:\"1,2,7,8\"}}",notifyUrl="/",key="test"}]    
        ///   }
        /// </returns>
        JsonResult SyncAlarm(string token, long timestamp, string sign);

        /// <summary>
        /// 上传APP版本号和包名
        /// </summary>
        /// <param name="versionName">版本名称</param>
        /// <param name="packageName">包名</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostAppVersion(string versionName, string packageName, string token, long timestamp, string sign);

        /// <summary>
        /// 检查APP更新版本
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     dataList=[
        ///         {versionName="1.0", packageName="com.lsinfo.maltose", versionCode="1", id="abcdefg1234"}
        ///     ]
        /// }
        /// </returns>
        JsonResult CheckUpgrade(string token, long timestamp, string sign);

        /// <summary>
        /// 上传系统信息
        /// </summary>
        /// <param name="equipmentInfo">设备系统信息</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostSystemInfo(EquipmentInfoDto equipmentInfo, string token, long timestamp, string sign);

        /// <summary>
        /// 上传系统播放信息
        /// </summary>
        /// <param name="playInfo">播放素材列表</param>
        /// <param name="playerPlayInfo">播放器与播放素材关系表</param>
        /// <param name="player">播放器列表</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostSystemPlayInfo(string playInfo, string playerPlayInfo, string player, string token, long timestamp, string sign);

        /// <summary>
        /// 上传网络状况
        /// </summary>
        /// <param name="equipmentNetworkInfo">设备网络信息</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostNetworkInfo(EquipmentNetworkInfoDto equipmentNetworkInfo, string token, long timestamp, string sign);

        /// <summary>
        /// 上传SDCard信息
        /// </summary>
        /// <param name="sdCardTotalMemory">总存储</param>
        /// <param name="sdCardAvailableMemory">可用存储</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostSDCardInfo(string sdCardTotalMemory, string sdCardAvailableMemory, string token, long timestamp, string sign);

        /// <summary>
        /// 上传内存信息
        /// </summary>
        /// <param name="totalRam">总内存</param>
        /// <param name="availableRam">可用内存</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult PostRamInfo(string totalRam, string availableRam, string token, long timestamp, string sign);        

        /// <summary>
        /// APP升级处理结果反馈
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        /// 
        JsonResult UpgradeNotify(string appId, bool success, string token, long timestamp, string sign);

        /// <summary>
        /// 上传截图
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult ScreenshotUpload(string token, long timestamp, string sign);

        /// <summary>
        /// 上传日志
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>
        ///  数据格式：
        /// {
        ///     code=1,
        ///     msg="成功"
        /// }
        /// </returns>
        JsonResult LogUpload(string token, long timestamp, string sign);

        /// <summary>
        /// 下载资源文件
        /// </summary>
        /// <param name="playInfoId">资源Id（方案Id）</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>资源文件数据流</returns>
        FileStreamResult DownloadResources(string playInfoId, string token, long timestamp, string sign);

        /// <summary>
        /// 下载App
        /// </summary>
        /// <param name="id">AppId</param>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="sign">数据签名</param>
        /// <returns>App文件数据流</returns>
        FileStreamResult DownloadApp(string id, string token, long timestamp, string sign);
    }
}
