using System;
using System.Collections.Generic;
using UNodes.Extensions;

namespace UNodes.Data
{
    public static class SaveData
    {
        public static bool NoAdsIsPurchased;
        public static DateTime SaveTime;

        private const string NO_ADS_IS_PURCHASED_KEY = "noAds";
        public const string SAVE_TIME_KEY = "saveTime";

        public static Dictionary<string, string> GetSaveData()
        {
            var saveData = new Dictionary<string, string>
            {
                { NO_ADS_IS_PURCHASED_KEY, NoAdsIsPurchased.ToSaveString() },
                { SAVE_TIME_KEY, SaveTime.ToSaveString() },
            };

            return saveData;
        }

        public static void RestoreFromSaveData(Dictionary<string, string> saveData)
        {
            NoAdsIsPurchased = saveData.GetValueOrDefault(NO_ADS_IS_PURCHASED_KEY, "0")
                .SaveStringToBool();

            SaveTime = saveData.GetValueOrDefault(SAVE_TIME_KEY, "0")
                .SaveStringToDateTime();
        }

    }
}
