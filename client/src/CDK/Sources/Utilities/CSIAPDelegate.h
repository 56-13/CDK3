#ifndef __CDK__CSIAPDelegate__
#define __CDK__CSIAPDelegate__

class CSIAPDelegate {
public:
    inline virtual void onIAPConnected() {}
    inline virtual void onIAPSyncProduct(const char* productId, const char* price, const char *originPrice, const char* currencyCode) {}
    inline virtual void onIAPSync() {}
    inline virtual void onIAPComplete(const char* productId, const char* chargeData1, const char* chargeData2, const char* payload) {}
    inline virtual void onIAPCancelled() {}
    inline virtual void onIAPError(int errorCode) {}
};

#endif
