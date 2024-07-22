using UnityEngine;

// Tạo một class mới có tên SpinObject
public class SpinObject : MonoBehaviour
{
    // Tốc độ quay của GameObject, có thể chỉnh sửa trong Inspector
    public float rotateSpeed = 100f;

    // Hàm Update được gọi mỗi khung hình (frame)
    void Update()
    {
        // Quay GameObject quanh trục Y (đứng) tại vị trí hiện tại với tốc độ được chỉ định
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}