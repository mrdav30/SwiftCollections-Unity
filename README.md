SwiftCollections-Unity
==============

![SwiftCollections Icon](https://raw.githubusercontent.com/mrdav30/SwiftCollections/main/icon.png)

**SwiftCollections** is a Unity package that provides high-performance, memory-efficient data structures optimized for game development and real-time applications. 

This package is a Unity-specific implementation of the [SwiftCollections](https://github.com/mrdav30/SwiftCollections) library

---

## 🛠️ Key Features

- **Optimized for Performance**: Designed for low time complexity and minimal memory allocations.
- **Versatile Use Cases**: Suitable for data structures in 3D environments and complex spatial queries.
- **Memory-Conscious Data Structures**: Custom allocators and lightweight collections minimize GC pressure.

---

## 🚀 Installation

### Via Unity Package Manager (UPM)

1. Open Unity and navigate to **Window → Package Manager**.
2. Click the **+** button and select **Add package from git URL**.
3. Enter the following URL:

https://github.com/mrdav30/SwiftCollections-Unity.git

4. Click Add and Unity will install the package automatically.

### Manual Installation

1. Download the .unitypackage file from the [latest release](https://github.com/mrdav30/SwiftCollections-Unity/releases).
2. Open Unity and import the package via **Assets → Import Package → Custom Package...**.
3. Select the downloaded file and import the contents.

---

## 🧩 Dependencies

SwiftCollections-Unity depends on the following Unity package:

- [FixedMathSharp-Unity](https://github.com/mrdav30/FixedMathSharp-Unity)

This dependency is automatically included when installing via UPM.

---

## 📖 Usage Examples

### SwiftBVH for Spatial Queries

```csharp
var bvh = new SwiftBVH<int>(100);
var volume = new BoundingVolume(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
bvh.Insert(1, volume);

var results = new List<int>();
bvh.Query(new BoundingVolume(new Vector3(0, 0, 0), new Vector3(2, 2, 2)), results);
Console.WriteLine(results.Count); // Output: 1
```

### SwiftArray2D

```csharp
var array2D = new Array2D<int>(10, 10);
array2D[3, 4] = 42;
Console.WriteLine(array2D[3, 4]); // Output: 42
```

### SwiftQueue

```csharp
var queue = new SwiftQueue<int>(10);
queue.Enqueue(5);
Console.WriteLine(queue.Dequeue()); // Output: 5
```

### Populating Arrays

```csharp
var array = new int[10].Populate(() => new Random().Next(1, 100));
```

## 🛠️ Compatibility

- **.NET Framework:** 4.7.2+
- **Unity3D Version:** 2022.3+
- **Platforms:** Windows, Linux, macOS, WebGL, Mobile

## 📄 License

This project is licensed under the MIT License - see the `LICENSE` file
for details.

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
