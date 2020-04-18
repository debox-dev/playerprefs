# DeBox PlayerPrefs

Helpful extensions for the Unity PlayerPrefs sub-system

## Installation instructions
- Open your project manifest file (`MyProject/Packages/manifest.json`).
- Add `"com.debox.teleport": "https://github.com/debox-dev/playerprefs.git"` to the `dependencies` list.
- Open or focus on Unity Editor to resolve packages.


## Requirements
- Unity 2019 or higher.

## Examples

```
using DeBox.PlayerPrefs;

public class MyClass
{
   private readonly PlayerPrefsInt _playerGold = new PlayerPrefsInt("gold", 0);

   public void GetGold()
   {
       return _playerGold.Value;
   }

   public void AddGold(int value):
   {
       _playerGold.Value += value;
   }

   public void ResetGold()
   {
       _playerGold.Delete();
       // _playerGold.Value will return 0 now
   }
}
```
