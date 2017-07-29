using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamControlium.Framework
{
    /// <summary>
    /// Collection of TeamControlium Controls previously found in the current test and used as a cache to minimise IP traffic when executing remotely.
    /// <para/><para/>
    /// Tests will routinely re-find previously located DOM elements when, for example, the screen is refreshed and Stale Elements need re-finding.  When items in the
    /// non-stale elements are reused IP traffic increases (and slows test execution).  Therfore, this collection is used as a Control cache so that when controls are located (Using the SetControl)
    /// the cache is checked and if there is a hit the Cache reference is passed back as the reference.  The time it takes to perform an in-memory lookup is very much quicker than to pass element data
    /// over the wire (both in terms of bandwidth and propergation delay) and hence caching.
    /// <para/><para/>
    /// Caching doesnt apply to a local execution and it is up to the calling class/s to decide if caching should be used locally.  However, as the overhead (in terms of memory useage and time) is small
    /// it probably doesnt matter.  It is probably best to use caching locally as well as it would highlight any caching related issues. </summary>
    public static class ControlCache
    {
        private static Collection<ControlBase> controls = new Collection<ControlBase>();
        public enum ControlCacheStates { CachingDisabled, CacheHit, CacheMiss, CachedControlWasStale };

        /// <summary>Setup caching defaults
        /// </summary>
        static ControlCache()
        {
            // Booleans default to false anyway - but lets be on the safe side....
            DisableControlCaching = false;
        }

        /// <summary>Enables or disables control caching - Defaults to false (IE. Control caching is on)
        /// </summary>
        /// <remarks>Tests may require control caching to be disabled during some activity.  This usually is required on pages that have alot of dynamic UI activity where caching may hide changes to elements not detected by Selenium.
        /// <para/><para/>If tests are aborting due to elements appearing to co-react (input goes to wrong control or click being missed) try disabling the caching in the specific area.
        /// <para/><para/>
        /// When caching is disabled, controls passed to <see cref="Add"/> and ignored and calls to Get result in the passed control being returned (even if it exists in the cache.
        /// <para/><para/>
        /// Cache is not flushed when disabled/enabled as the coherency of the cache cannot be invalidated during a disabled phase.
        /// </remarks>
        public static bool DisableControlCaching { get; set; }

        /// <summary>Finds out if control already exists in the control collection
        /// <para/><para/>Tests should not have to call this method directly as Core Controls used &amp; reference the cache during control root element identification.
        /// </summary>
        /// <param name="NewControl">TeamControlium Control to be checked to see if it is already known about</param>
        /// <returns>True if the control exists, otherwise False</returns>
        /// <remarks>Controls are matched using string compares with Find Logic and Friendly Names.<note type="note">Parentage of controls is NOT currently taken into consideration</note></remarks>
        public static bool Exists(ControlBase NewControl)
        {
            if (DisableControlCaching) return false;
            foreach (ControlBase collectionControl in controls)
            {
                if (ControlMatch(NewControl, collectionControl) == true)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>Adds a new Control to the library
        /// <para/><para/>Tests should not have to call this method directly as Core Controls used &amp; reference the cache during control root element identification.
        /// </summary>
        /// <param name="NewControl">TeamControlium Control to add to the Cache</param>
        /// <remarks>If <see cref="DisableControlCaching"/> is true Control is not added to cache.  If Control already exists in the cache an exception is thrown</remarks>
        public static void Add(ControlBase NewControl)
        {
            if (DisableControlCaching) return;
            // Its assumed we are only calling Add if we have checked the control doesnt exist.  But, lets make sure......
            if (!Exists(NewControl))
            {
                controls.Add(NewControl);
            }
            else
            {
                throw new Exception("FATAL - Control [" + NewControl.Mapping?.FriendlyName ?? NewControl.Mapping?.FindLogic ?? "Unknown!!  No find login or friendly name" + "] already exists - why did we not reuse it (on invalidate if stale?)?!");
            }
        }
        /// <summary>Get control from Cache library
        /// </summary>
        /// <param name="FoundControlToGet">Control to get</param>
        /// <returns>Cached control's reference.  If DisableControlCaching is true, the passed FoundControlToGet is returned.</returns>
        public static ControlBase Get(ControlBase FoundControlToGet)
        {
            // If we are not caching, just return the passed control....
            if (DisableControlCaching) return FoundControlToGet;
            foreach (ControlBase collectionControl in controls)
            {
                if (ControlMatch(FoundControlToGet, collectionControl) == true) return collectionControl;
            }
            throw new Exception("CoreLib_Control:ControlCollection - cannot locate control: " + FoundControlToGet.Mapping?.FriendlyName ?? FoundControlToGet.Mapping?.FindLogic ?? "Unknown!!  No find login or friendly name");
        }

        /// <summary>
        /// Checks to see if the control referenced exists in the cache.  If it does, the reference is updated and true is returned.  Otherwise, it is added and false returned.
        /// <note type="note">If Caching is disabled (See <see cref="DisableControlCaching"/>) nothing is done (no reference change or added) and false returned (even if the control exists in the cache). </note>
        /// </summary>
        /// <param name="ControlToCheck">Control to be put throuch caching logic</param>
        /// <returns>True if cache hit and reference updated, false if added to cache and reference not changed</returns>
        public static ControlCacheStates Check<T>(ref T ControlToCheck) where T : ControlBase
        {
            return ControlCacheStates.CachingDisabled; // We have a problem...
            //
            // Scenario - we set a control and then a bunch of children from it.
            //            If 1 child is stalethe whole cache - parent and siblings are flushed!!!
            //


            if (DisableControlCaching) return ControlCacheStates.CachingDisabled;
            if (Exists(ControlToCheck))
            {
                ControlBase CachedControl = Get(ControlToCheck);
                if (CachedControl.IsStale)
                {
                    //
                    // Woah!!!  Element is stale so invalidate the whole cache!, add the new element and return...
                    //
                    ClearCache();
                    Add(ControlToCheck);
                    return ControlCacheStates.CachedControlWasStale;
                }
                else
                {
                    // Ok, we have hit and it is NOT stale; it is in the cache so reference the Cache control and let caller know...
                    ControlToCheck = (T)CachedControl;
                    return ControlCacheStates.CacheHit;
                }
            }
            else
            {
                // Doesnt exist in the cache.  So add it and let caller know
                Add(ControlToCheck);
                return ControlCacheStates.CacheMiss;
            }
        }


        public static void ClearCache()
        {
            controls.Clear();
        }

        private static bool Compare(string A, string B)
        {
            if (string.IsNullOrEmpty(A) && !string.IsNullOrEmpty(B)) return false;
            if (string.IsNullOrEmpty(B) && !string.IsNullOrEmpty(A)) return false;
            if (string.IsNullOrEmpty(A) && string.IsNullOrEmpty(B)) return true;
            return A.Equals(B);
        }
        private static bool ControlMatch(ControlBase Control1, ControlBase Control2)
        {
            ControlBase _control1;
            ControlBase _control2;

            if (Compare(Control1?.Mapping?.FindLogic?.ToString() ?? null, Control2?.Mapping?.FindLogic?.ToString() ?? null))
            {
                _control1 = Control1;
                _control2 = Control2;

                // So, the controls find logic mapped, now we iterate up the tree to check if their ancestors are common....
                while (_control1.ParentControl != null)
                {
                    if (_control2.ParentControl == null)
                    {
                        return false;  // Control1 has a parent but Control 2 doesn't - so they cannot be the same control....
                    }
                    else
                    {
                        if (!Compare(_control1?.Mapping?.FindLogic?.ToString() ?? null, _control2?.Mapping?.FindLogic?.ToString() ?? null))
                        {
                            return false; // Their ancestors, at this level (Parent, Grandpartent etc...) dont match so they are not the same...
                        }
                        else
                        {
                            //
                            // They match at this level, so uo we go to the parents.
                            //
                            _control1 = _control1.ParentControl;
                            _control2 = _control2.ParentControl;
                        }
                    }
                }
                if (_control2.ParentControl != null)
                {
                    return false;  // Control1 (or an ancestor) is null, or we wouldnt be here, so if Control2 (or an ancestor on the same level) is NOT null we dont have a match... 
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }
    }
}