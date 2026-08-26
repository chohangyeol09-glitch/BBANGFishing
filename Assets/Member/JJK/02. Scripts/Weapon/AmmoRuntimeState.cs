namespace Member.JJK._02._Scripts.Weapon
{
    public class AmmoRuntimeState
    {
        public int currentAmmo;
        public bool isReloading;
        
        public AmmoRuntimeState(WeaponSO weaponData)
        {
            currentAmmo = weaponData.MagazineSize;
        }
    }
}