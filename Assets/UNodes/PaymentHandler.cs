using UNodes.Data;

namespace UNodes.Project
{
    public class PaymentHandler :  BasePaymentHandler
    {
        public override void ApplyPurchaseReward(ProductKey productKey)
        {
            switch (productKey)
            {
                case ProductKey.NoAds:
                    SaveData.NoAdsIsPurchased = true;
                    break;

                default:
                    break;
            }
        }

    }
}
