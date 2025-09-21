# 通用工具类 (Utility)

<cite>
**Referenced Files in This Document**   
- [Utility.Text.cs](file://GameFramework/Utility/Utility.Text.cs)
- [Utility.Json.cs](file://GameFramework/Utility/Utility.Json.cs)
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs)
- [Utility.Compression.cs](file://GameFramework/Utility/Utility.Compression.cs)
- [Utility.Encryption.cs](file://GameFramework/Utility/Utility.Encryption.cs)
- [Utility.Verifier.cs](file://GameFramework/Utility/Utility.Verifier.cs)
- [Utility.Random.cs](file://GameFramework/Utility/Utility.Random.cs)
- [Utility.Converter.cs](file://GameFramework/Utility/Utility.Converter.cs)
</cite>

## 目录
1. [简介](#简介)
2. [核心工具模块](#核心工具模块)
   - [字符串处理 (Utility.Text)](#字符串处理-utilitytext)
   - [JSON序列化 (Utility.Json)](#json序列化-utilityjson)
   - [路径操作 (Utility.Path)](#路径操作-utilitypath)
   - [数据压缩 (Utility.Compression)](#数据压缩-utilitycompression)
   - [数据加密 (Utility.Encryption)](#数据加密-utilityencryption)
   - [校验和计算 (Utility.Verifier)](#校验和计算-utilityverifier)
   - [随机数生成 (Utility.Random)](#随机数生成-utilityrandom)
   - [类型转换 (Utility.Converter)](#类型转换-utilityconverter)
3. [设计特点与最佳实践](#设计特点与最佳实践)

## 简介

`Utility` 命名空间是游戏框架的核心工具库，提供了一系列静态、无状态的实用方法，旨在简化开发流程、提高代码质量和运行性能。这些工具类通过封装常见的编程任务，为开发者提供了高效、可靠的解决方案。所有工具方法均设计为线程安全的静态方法，无需实例化即可直接调用，极大地提升了代码的可读性和可维护性。

**Section sources**
- [Utility.Text.cs](file://GameFramework/Utility/Utility.Text.cs#L1-L622)
- [Utility.Json.cs](file://GameFramework/Utility/Utility.Json.cs#L1-L120)

## 核心工具模块

### 字符串处理 (Utility.Text)

`Utility.Text` 模块提供了强大的字符串格式化功能，支持多达16个泛型参数的格式化操作。该模块通过 `ITextHelper` 接口实现了可扩展性，允许开发者根据需要替换底层的格式化实现。

核心方法包括：
- `Format<T>(string format, T arg)`：单参数格式化
- `Format<T1, T2>(string format, T1 arg1, T2 arg2)`：双参数格式化
- 支持最多16个参数的 `Format` 重载方法

这些方法在内部优先使用 `ITextHelper` 实现，若未设置则回退到 .NET 原生的 `string.Format` 方法。所有方法均包含参数验证，确保输入的格式字符串不为空。

**Section sources**
- [Utility.Text.cs](file://GameFramework/Utility/Utility.Text.cs#L1-L622)

### JSON序列化 (Utility.Json)

`Utility.Json` 模块提供了简洁的JSON序列化与反序列化接口，通过 `IJsonHelper` 接口实现底层JSON库的解耦。这种设计允许框架在不修改调用代码的情况下更换JSON处理库。

主要功能包括：
- `ToJson(object obj)`：将对象序列化为JSON字符串
- `ToObject<T>(string json)`：将JSON字符串反序列化为指定类型的对象
- `ToObject(Type objectType, string json)`：将JSON字符串反序列化为指定类型的对象（通过Type参数）

该模块包含完善的异常处理机制，能够捕获并包装底层JSON库的异常，提供更清晰的错误信息。

**Section sources**
- [Utility.Json.cs](file://GameFramework/Utility/Utility.Json.cs#L1-L120)

### 路径操作 (Utility.Path)

`Utility.Path` 模块专注于跨平台的路径处理，解决了不同操作系统间路径分隔符不一致的问题。

主要方法包括：
- `GetRegularPath(string path)`：将路径中的反斜杠 `\` 统一替换为正斜杠 `/`，确保路径格式的一致性
- `GetRemotePath(string path)`：将普通路径转换为远程格式路径（添加 `file:///` 前缀）
- `RemoveEmptyDirectory(string directoryName)`：递归删除空文件夹，清理项目中的冗余目录

这些方法特别适用于需要在不同平台间共享资源路径的场景。

**Section sources**
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L1-L101)

### 数据压缩 (Utility.Compression)

`Utility.Compression` 模块提供了完整的数据压缩与解压功能，支持字节数组和流两种数据形式。通过 `ICompressionHelper` 接口实现压缩算法的可替换性。

核心功能包括：
- `Compress(byte[] bytes)`：将字节数组压缩为新的字节数组
- `Compress(byte[] bytes, Stream compressedStream)`：将字节数组压缩到指定流中
- `Decompress(byte[] bytes)`：将压缩的字节数组解压为新的字节数组
- `Decompress(Stream stream, Stream decompressedStream)`：从流中读取压缩数据并解压到另一个流

该模块支持多种重载形式，既可以直接获取压缩结果，也可以通过流进行高效处理，避免大文件处理时的内存压力。

**Section sources**
- [Utility.Compression.cs](file://GameFramework/Utility/Utility.Compression.cs#L1-L345)

### 数据加密 (Utility.Encryption)

`Utility.Encryption` 模块提供了基于异或(XOR)运算的快速数据加密功能，适用于简单的数据混淆和保护。

主要方法包括：
- `GetXorBytes(byte[] bytes, byte[] code)`：使用指定密钥对数据进行异或加密
- `GetSelfXorBytes(byte[] bytes, byte[] code)`：在原数组上进行异或操作，节省内存分配
- `GetQuickXorBytes(byte[] bytes, byte[] code)`：仅对数据的前220字节进行快速加密
- `GetQuickSelfXorBytes(byte[] bytes, byte[] code)`：在原数组上对前220字节进行快速加密

这些方法支持任意长度的密钥，通过循环使用密钥字节实现数据混淆，适用于资源文件的简单加密保护。

**Section sources**
- [Utility.Encryption.cs](file://GameFramework/Utility/Utility.Encryption.cs#L1-L135)

### 校验和计算 (Utility.Verifier)

`Utility.Verifier` 模块提供了CRC32校验和计算功能，用于数据完整性验证。

主要功能包括：
- `GetCrc32(byte[] bytes)`：计算字节数组的CRC32校验和
- `GetCrc32(Stream stream)`：计算数据流的CRC32校验和
- `GetCrc32(byte[] bytes, int offset, int length)`：计算字节数组指定范围的CRC32校验和
- `GetCrc32Bytes(int crc32)`：将CRC32值转换为字节数组

该模块使用内部缓存机制处理大文件，避免一次性加载整个文件到内存，同时确保计算完成后正确重置算法状态。

**Section sources**
- [Utility.Verifier.cs](file://GameFramework/Utility/Utility.Verifier.cs#L1-L196)

### 随机数生成 (Utility.Random)

`Utility.Random` 模块封装了.NET的随机数生成器，提供了更简洁的接口。

主要方法包括：
- `SetSeed(int seed)`：设置随机数种子，实现可重现的随机序列
- `GetRandom()`：生成非负随机整数
- `GetRandom(int maxValue)`：生成指定范围内的随机整数
- `GetRandom(int minValue, int maxValue)`：生成指定区间内的随机整数
- `GetRandomDouble()`：生成0.0到1.0之间的随机浮点数
- `GetRandomBytes(byte[] buffer)`：用随机数填充字节数组

该模块使用UTC时间戳作为默认种子，确保每次运行程序时生成不同的随机序列。

**Section sources**
- [Utility.Random.cs](file://GameFramework/Utility/Utility.Random.cs#L1-L80)

### 类型转换 (Utility.Converter)

`Utility.Converter` 模块提供了丰富的类型转换功能，特别是字节数组与其他基本类型之间的转换。

主要功能包括：
- 基本类型与字节数组的相互转换（`GetBytes` 和 `GetXXX` 系列方法）
- 字符串与字节数组的编码转换（支持UTF-8等编码）
- 长度单位转换（像素、厘米、英寸之间的相互转换）
- 字节序检测（`IsLittleEndian` 属性）

该模块特别适用于网络通信、文件读写等需要处理原始字节数据的场景，提供了安全的边界检查和错误处理。

**Section sources**
- [Utility.Converter.cs](file://GameFramework/Utility/Utility.Converter.cs#L1-L849)

## 设计特点与最佳实践

`Utility` 命名空间的设计体现了以下几个重要特点：

1. **静态无状态**：所有工具类均为静态类，不维护任何实例状态，确保线程安全和内存效率。

2. **接口抽象**：关键功能（如文本、JSON、压缩）通过接口（`ITextHelper`、`IJsonHelper`、`ICompressionHelper`）实现，支持运行时替换底层实现。

3. **错误处理**：统一使用 `GameFrameworkException` 包装底层异常，提供一致的错误处理机制。

4. **性能优化**：采用内存复用（如 `GetSelfXorBytes`）、流式处理等技术，减少内存分配和提高处理效率。

5. **边界检查**：所有方法都包含参数验证，防止越界访问和空引用异常。

在使用这些工具类时，建议：
- 在程序启动时通过 `SetXXXHelper` 方法配置合适的辅助器实现
- 对于大文件处理，优先使用流式API避免内存溢出
- 在需要可重现结果的场景中，使用 `SetSeed` 固定随机数种子
- 利用类型转换工具处理跨平台数据交换

这些工具类共同构成了框架高效运行的基石，为开发者提供了强大而易用的编程支持。

**Section sources**
- [Utility.Text.cs](file://GameFramework/Utility/Utility.Text.cs#L1-L622)
- [Utility.Json.cs](file://GameFramework/Utility/Utility.Json.cs#L1-L120)
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L1-L101)
- [Utility.Compression.cs](file://GameFramework/Utility/Utility.Compression.cs#L1-L345)
- [Utility.Encryption.cs](file://GameFramework/Utility/Utility.Encryption.cs#L1-L135)
- [Utility.Verifier.cs](file://GameFramework/Utility/Utility.Verifier.cs#L1-L196)
- [Utility.Random.cs](file://GameFramework/Utility/Utility.Random.cs#L1-L80)
- [Utility.Converter.cs](file://GameFramework/Utility/Utility.Converter.cs#L1-L849)