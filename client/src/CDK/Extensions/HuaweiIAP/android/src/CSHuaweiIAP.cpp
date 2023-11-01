
#ifdef CDK_ANDROID

#define CDK_IMPL

#include "CSHuaweiIAP.h"

#include "CSJNI.h"

static CSIAPDelegate* __delegate = NULL;

extern "C"
{
    void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativeConnected(JNIEnv* env, jclass clazz) {
        if (__delegate) {
            __delegate->onIAPConnected();
        }
    }

void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativeSyncProduct(JNIEnv* env, jclass clazz, jstring jproductId, jstring jprice, jstring joriginPrice, jstring jcurrencyCode) {
    if (__delegate) {
        const char* productId = env->GetStringUTFChars(jproductId, NULL);
        const char* price = env->GetStringUTFChars(jprice, NULL);
        const char* originPrice= env->GetStringUTFChars(joriginPrice, NULL);
        const char* currencyCode = env->GetStringUTFChars(jcurrencyCode, NULL);

        __delegate->onIAPSyncProduct(productId, price, originPrice, currencyCode);

        env->ReleaseStringUTFChars(jproductId, productId);
        env->ReleaseStringUTFChars(jprice, price);
        env->ReleaseStringUTFChars(joriginPrice, originPrice);
        env->ReleaseStringUTFChars(jcurrencyCode, currencyCode);
    }
}

void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativeSync(JNIEnv* env, jclass clazz) {
    if (__delegate) {
        __delegate->onIAPSync();
    }
}

    void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativePurchaseComplete(JNIEnv *env, jclass clazz, jstring jproductId, jstring jchargeData1, jstring jchargeData2, jstring jpayload) {
        if (__delegate) {
            const char* productId = env->GetStringUTFChars(jproductId, NULL);
            const char* chargeData1 = env->GetStringUTFChars(jchargeData1, NULL);
            const char* chargeData2 = env->GetStringUTFChars(jchargeData2, NULL);
            const char* payload = env->GetStringUTFChars(jpayload, NULL);
            
            __delegate->onIAPComplete(productId, chargeData1, chargeData2, payload);
            
            env->ReleaseStringUTFChars(jproductId, productId);
            env->ReleaseStringUTFChars(jchargeData1, chargeData1);
            env->ReleaseStringUTFChars(jchargeData2, chargeData2);
            env->ReleaseStringUTFChars(jpayload, payload);
        }
    }
    
    void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativePurchaseCancelled(JNIEnv* env, jclass clazz) {
        if (__delegate) {
            __delegate->onIAPCancelled();
        }
    }
    
    void Java_kr_co_brgames_cdk_extension_CSHuaweiIAP_nativeError(JNIEnv* env, jclass clazz, jint errorCode) {
        if (__delegate) {
            __delegate->onIAPError(errorCode);
        }
    }
}

void CSHuaweiIAP::initialize() {
    CSJNIMethod mi;
    CSJNI::findMethod(mi, CSJNIClassHuaweiIAP, "initialize", "()V", false);
    mi.env->CallStaticVoidMethod(mi.classId, mi.methodId);
}

void CSHuaweiIAP::finalize() {
    CSJNIMethod mi;
    CSJNI::findMethod(mi, CSJNIClassHuaweiIAP, "dispose", "()V", false);
    mi.env->CallStaticVoidMethod(mi.classId, mi.methodId);
}

void CSHuaweiIAP::setDelegate(CSIAPDelegate* delegate) {
    __delegate = delegate;
}

void CSHuaweiIAP::connect() {
    CSJNIMethod mi;
    CSJNI::findMethod(mi, CSJNIClassHuaweiIAP, "connect", "()V", false);
    mi.env->CallStaticVoidMethod(mi.classId, mi.methodId);
}

void CSHuaweiIAP::sync(const char* const* productIds, uint count) {
    CSJNIMethod mi;
    CSJNI::findMethod(mi, CSJNIClassHuaweiIAP, "sync", "([Ljava/lang/String;)V", false);
    jobjectArray jproductIds = (jobjectArray)mi.env->NewObjectArray(count, mi.env->FindClass("java/lang/String"), NULL);
    for (int i = 0; i < count; i++) {
        mi.env->SetObjectArrayElement(jproductIds, i, mi.env->NewStringUTF(productIds[i]));
    }
    mi.env->CallStaticVoidMethod(mi.classId, mi.methodId, jproductIds);
    mi.env->DeleteLocalRef(jproductIds);
}

void CSHuaweiIAP::purchase(const char* productId, const char* payload) {
    CSJNIMethod mi;
    CSJNI::findMethod(mi, CSJNIClassHuaweiIAP, "purchase", "(Ljava/lang/String;Ljava/lang/String;)V", false);
    jstring jproductId = mi.env->NewStringUTF(productId);
    jstring jpayload = mi.env->NewStringUTF(payload);
    mi.env->CallStaticVoidMethod(mi.classId, mi.methodId, jproductId, jpayload);
    mi.env->DeleteLocalRef(jproductId);
    mi.env->DeleteLocalRef(jpayload);
}

#endif