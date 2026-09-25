# Unity Base Test - 3D Third-Person Platformer & Interaction Framework

[![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f2%20LTS-blue.svg?logo=unity)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-PC%20%2F%20Standalone-brightgreen.svg)]()
[![Language](https://img.shields.io/badge/C%23-10.0%20%2F%20.NET%20Standard%202.1-purple.svg?logo=csharp)]()

---

## 🎮 Tính năng nổi bật (Features)

### 1. Điều khiển & Vật lý nhân vật (Player Controller & Game Feel)
- **CharacterController & Camera-Relative Movement**: Nhân vật di chuyển mượt mà và tự động xoay hướng theo góc nhìn Camera (`ThirdPersonCamera.PlanarForward / PlanarRight`).
- **Nâng cao Game Feel**:
  - **Jump Buffer (`jumpBufferTime = 0.15s`)**: Ghi nhớ phím nhảy trước khi chạm đất, tránh mất phản hồi khi bấm sớm.
  - **Coyote Time (`coyoteTime = 0.12s`)**: Cho phép nhảy trong khoảng thời gian ngắn sau khi vừa rời khỏi mép rìa sàn.
  - **Ground Detection kết hợp**: Kết hợp giữa `CharacterController.isGrounded` và `Physics.Raycast` để phát hiện mặt đất ổn định, mượt mà.
- **Hệ thống Animation điều khiển theo sự kiện**: Liên kết chặt chẽ giữa trạng thái di chuyển, nhảy, chạm đất và tử vong với `Animator`.

### 2. Camera góc nhìn thứ 3 thông minh (Smart Orbit Camera)
- **Xoay & Zoom mượt mà**: Hỗ trợ xoay quanh nhân vật bằng thao tác giữ chuột trái, zoom khoảng cách qua cuộn chuột (Scroll Wheel).
- **Chống xuyên tường/sàn (Anti-Clipping SphereCast)**: Tự động phát hiện va chạm vật cản bằng `Physics.SphereCast` và điều chỉnh khoảng cách tức thì để tránh camera đâm xuyên vật thể trong màn chơi.
- **Tối ưu UI Pointer**: Tự động bỏ qua xoay camera khi trỏ chuột đang nằm trên các phần tử giao diện người dùng (UI Elements).

### 3. Hệ thống tương tác dạng Module (Modular Interaction System)
- **Giao diện `IInteractable`**: Cho phép dễ dàng tạo bất kỳ vật thể tương tác nào (Cửa, Nút bấm,) chỉ bằng việc kế thừa interface.
- **Tối ưu hóa hiệu năng (`Physics.OverlapSphereNonAlloc`)**: Quét vùng xung quanh nhân vật để tìm vật thể gần nhất mà không gây phân mảnh bộ nhớ (GC Alloc).
- **UI Billboard động**: Nhãn gợi ý phím `[E]` hiển thị phía trên vật thể và luôn tự động xoay mặt về phía Camera (`Billboard.cs`).
- **Nút bấm vật lý (Button Press Feedback)**: Hỗ trợ hiệu ứng thụt nút bấm khi nhấn và kích hoạt sự kiện `UnityEvent`.
- **Cửa trượt/quay thông minh (`DoorController`)**: Hỗ trợ xoay mở cửa mượt mà qua Coroutine và tự động ngắt collider cản đường.

### 4. Cơ quan Platform chuyển động tuần tự (Sequential Dynamic Platforms)
- **`PlatformGroupController`**: Cho phép kích hoạt chuỗi bậc thang/cầu nối xuất hiện tuần tự theo hiệu ứng chuyển động mượt.
- **Preset đa dạng**: Tích hợp sẵn nhiều hướng thu gọn/mở rộng (`DownIntoPit`, `IntoWallRight`, `IntoWallLeft`, `IntoWallBack`, `IntoWallFront` hoặc `CustomOffset`).
- **Hỗ trợ Gizmos trong Editor**: Hiển thị trực quan quỹ đạo chuyển động của các bậc platform trong cửa sổ Scene.

### 5. Vùng bẫy & Vòng lặp trò chơi (Hazard & Game Loop)
- **`HazardZone`**: Vùng bẫy nguy hiểm kích hoạt trạng thái tử vong cho nhân vật khi rơi vào.
- **`GameManager` (Singleton)**: Quản lý vòng đời trò chơi (`Playing`, `GameOver`, `Victory`), lắng nghe sự kiện chết của nhân vật và hỗ trợ tải lại màn chơi (`RestartScene`).

### 6. Công cụ Editor hỗ trợ (Custom Editor Utilities)
- **Batch Add MeshColliders**: Menu `Tools/Map/Thêm MeshCollider vào tất cả vật thể đang chọn` cho phép quét nhanh toàn bộ mesh con của GameObject được chọn và tự động bổ sung `MeshCollider` (có hỗ trợ tính năng Undo).

---

## ⌨️ Bảng điều khiển (Controls)

| Phím / Thao tác | Hành động |
| :--- | :--- |
| <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> / Phím mũi tên | Di chuyển nhân vật theo hướng nhìn của camera |
| <kbd>Space</kbd> | Nhảy (Hỗ trợ Jump Buffer & Coyote Time) |
| <kbd>E</kbd> hoặc Click Chuột Trái vào vật thể | Tương tác với cơ quan (Mở cửa, nhấn nút...) |
| Giữ <kbd>Chuột Trái</kbd> + Kéo | Xoay góc nhìn Camera 360° xung quanh nhân vật |
| <kbd>Cuộn Chuột (Scroll Wheel)</kbd> | Thu phóng khoảng cách Camera (Zoom In / Out) |

---

## 📁 Cấu trúc thư mục (Directory Structure)

```text
Assets/
├── Animations/             # FBX Animation (Idle, Slow Run, Jumping, Death) & Animator Controller
├── GraphicAssets/          # Model 3D (Char9, MapSchool với 1.fbx, 2.fbx) & Textures
├── Prefabs/                # Các Prefab hoàn chỉnh:
│   ├── Character.prefab    # Prefab nhân vật chính (đầy đủ controller, input, animation)
│   ├── Door.prefab         # Prefab cửa tương tác
│   └── Text_Prompt_E.prefab # Prefab UI gợi ý phím [E] (World Space)
├── Scenes/                 # Thư mục chứa Scene
│   ├── GameScence.unity    # Scene màn chơi chính (Gameplay)
│   └── SampleScene.unity   # Scene thử nghiệm
├── Scripts/                # Toàn bộ mã nguồn C# của dự án
│   ├── Camera/
│   │   └── ThirdPersonCamera.cs         # Script điều khiển Camera góc nhìn thứ 3
│   ├── Editor/
│   │   └── AddCollidersMenu.cs          # Tool mở rộng trong Unity Editor
│   ├── Interaction/
│   │   ├── Billboard.cs                 # Xoay UI luôn hướng về Camera
│   │   ├── DoorController.cs            # Điều khiển logic xoay & mở cửa
│   │   ├── HazardZone.cs                # Vùng nguy hiểm gây chết nhân vật
│   │   ├── IInteractable.cs             # Interface định nghĩa vật thể tương tác
│   │   ├── InteractableObject.cs        # Nút bấm/vật thể tương tác kèm animation
│   │   ├── PlatformGroupController.cs   # Điều khiển nhóm cầu/bậc thang di chuyển
│   │   └── PlayerInteractor.cs          # Quét và xử lý tương tác của người chơi
│   ├── Player/
│   │   ├── PlayerAnimation.cs           # Cầu nối giữa Input/Movement và Animator
│   │   ├── PlayerInput.cs               # Đọc và quản lý toàn bộ Input từ người chơi
│   │   └── PlayerMovement.cs            # Xử lý vật lý di chuyển, trọng lực, nhảy
│   └── GameManager.cs                   # Quản lý trạng thái trò chơi (Singleton)
└── TextMesh Pro/           # Cấu hình và tài nguyên của TextMeshPro
```
