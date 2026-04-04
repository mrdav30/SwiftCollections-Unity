# SwiftCollections-Unity

![SwiftCollections Icon](https://raw.githubusercontent.com/mrdav30/SwiftCollections/main/icon.png)

**SwiftCollections** is a Unity package that provides high-performance, memory-efficient data structures optimized for game development and real-time applications.

This package is a Unity-specific implementation of the [SwiftCollections](https://github.com/mrdav30/SwiftCollections) library.

---

## 🛠️ Key Features

- **Optimized for Performance**: Designed for low time complexity and minimal memory allocations.
- **Versatile Use Cases**: Suitable for data structures in 3D environments and complex spatial queries.
- **Memory-Conscious Data Structures**: Custom allocators and lightweight collections minimize GC pressure.

---

## 🧩 Dependencies

SwiftCollections-Unity depends on the following Unity package:

- [FixedMathSharp-Unity](https://github.com/mrdav30/FixedMathSharp-Unity)

This dependency will be installed automatically by SwiftCollections-Unity.

---

## 🚀 Installation

### Quick Install

1. Open **Package Manager → + → Add package from Git URL** and paste:
    <https://github.com/mrdav30/SwiftCollections-Unity.git>

### Manual Installation

1. Download `.unitypackage` file from the [latest release](https://github.com/mrdav30/SwiftCollections-Unity/releases).
2. Import the package via **Assets → Import Package → Custom Package...**.

Dependencies install automatically.

If Unity shows compile errors after install, run **Tools → mrdav30 → Repair com.mrdav30.swiftcollections Dependencies**

---

## 🛠️ Compatibility

- **.NET Standard:** 2.1
- **Unity3D Version:** 2022.3+
- **Platforms:** Windows, Linux, macOS, WebGL, Mobile

---

## ♻️ GameObject Pooling

This package includes a lightweight `GameObject` pool under `Runtime/GameObjectPool`.

### Setup

1. Create a pool asset via **Assets → Create → Utilities → ScriptableObjectPooler**.
2. Save it as `Resources/SwiftGameObjectPoolAsset.asset` so `SwiftGameObjectPoolManager.Shared` can load it.
3. Add one or more pool entries and configure:
   `Pool Name` — string ID used at runtime.
   `Prefab` — the object to spawn.
   `Budget` — max live instances for that pool.
   `Prewarm` — optionally create the full pool up front.

### Usage

```csharp
using SwiftCollections.Pool;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private const string BulletPoolId = "Bullet";

    public void Fire(Vector3 position, Quaternion rotation)
    {
        if (!SwiftGameObjectPoolManager.Shared.TryGetObject(BulletPoolId, out GameObject bullet))
            return;

        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.SetActive(true);
    }

    public void ReturnBullet(GameObject bullet)
    {
        SwiftGameObjectPoolManager.Shared.ReleaseObject(BulletPoolId, bullet);
    }
}
```

`TryGetObject(...)` returns `false` when the pool name is missing or every instance is currently checked out. If you prefer a throwing API, `GetObject(...)` is also available. Both methods return the pooled instance inactive, so configure it and enable it when ready. When you are finished with an instance, return it with `ReleaseObject(...)` so it can be reused.

## 📄 License

This project is licensed under the MIT License.

See the following files for details:

- LICENSE – standard MIT license
- NOTICE – additional terms regarding project branding and redistribution
- COPYRIGHT – authorship information

---

## 👥 Contributors

- **mrdav30** - Lead Developer
- Contributions are welcome! Feel free to submit pull requests or report issues.

---

## 💬 Community & Support

For questions, discussions, or general support, join the official Discord community:

👉 **[Join the Discord Server](https://discord.gg/mhwK2QFNBA)**

For bug reports or feature requests, please open an issue in this repository.

We welcome feedback, contributors, and community discussion across all projects.
