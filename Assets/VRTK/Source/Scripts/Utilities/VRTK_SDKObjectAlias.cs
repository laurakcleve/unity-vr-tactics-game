// SDK Object Alias|Utilities|90140
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The GameObject that the SDK Object Alias script is applied to will become a child of the selected SDK Object.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_SDKObjectAlias")]
    public class VRTK_SDKObjectAlias : MonoBehaviour
    {
        /// <summary>
        /// Valid SDK Objects
        /// </summary>
        public enum SDKObject
        {
            /// <summary>
            /// The main camera rig/play area object that defines the player boundary.
            /// </summary>
            Boundary,
            /// <summary>
            /// The main headset camera defines the player head.
            /// </summary>
            Headset
        }

        [Tooltip("The specific SDK Object to child this GameObject to.")]
        public SDKObject sdkObject = SDKObject.Boundary;

        protected VRTK_SDKManager sdkManager;

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged += LoadedSetupChanged;
            }
            ChildToSDKObject();
        }

        protected virtual void OnDisable()
        {
            if (sdkManager != null && !gameObject.activeSelf)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (sdkManager != null && gameObject.activeInHierarchy)
            {
                ChildToSDKObject();
            }
        }

        protected virtual void ChildToSDKObject()
        {
            Vector3 currentPosition = transform.localPosition;
            Quaternion currentRotation = transform.localRotation;
            Vector3 currentScale = transform.localScale;
            Transform newParent = null;

            switch (sdkObject)
            {
                case SDKObject.Boundary:
                    newParent = VRTK_DeviceFinder.PlayAreaTransform();
                    break;
                case SDKObject.Headset:
                    newParent = VRTK_DeviceFinder.HeadsetTransform();
                    break;
            }

            transform.SetParent(newParent);
            transform.localPosition = currentPosition;
            transform.localRotation = currentRotation;
            transform.localScale = currentScale;
        }
    }
}