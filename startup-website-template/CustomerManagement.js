var apiUrl = "https://localhost:7072/api/CustomerManagement/getallorder";
var ordersData = [];

// دالة لجلب البيانات من الـ API وعرضها على الصفحة
async function getAllOrders() {
  try {
    // استدعاء API باستخدام fetch
    var response = await fetch(apiUrl);

    // التحقق من نجاح الطلب
    if (!response.ok) {
      throw new Error("Failed to fetch data from the API");
    }

    // تحويل الاستجابة إلى JSON
    ordersData = await response.json();

    // عرض البيانات في الجدول
    displayOrders(ordersData);
  } catch (error) {
    console.error("Error:", error);
    alert("Error fetching the orders.");
  }
}

// دالة لعرض البيانات في الجدول
function displayOrders(data) {
  var tableBody = document.getElementById("ordersTableBody");
  tableBody.innerHTML = ""; // مسح المحتوى السابق

  // تكرار البيانات وعرضها في الجدول
  data.forEach((order) => {
    // إنشاء صف لكل طلب
    var orderRow = document.createElement("tr");

    // إضافة بيانات الطلب إلى الصف
    orderRow.innerHTML = `
            <td>${order.userFullName}</td>
            <td>${order.email || ""}</td>
            <td>${order.phoneNumber || ""}</td>
            <td>${order.totalOrders}</td>
            <td>${new Date(
              order.orders[0].requestdate
            ).toLocaleDateString()}</td>
        `;

    // إضافة الصف إلى جسم الجدول
    tableBody.appendChild(orderRow);
  });
}

// دالة البحث
function searchOrders() {
  var input = document.getElementById("searchInput").value.toLowerCase();
  var filteredData = ordersData.filter((order) => {
    return (
      order.userFullName.toLowerCase().includes(input) ||
      (order.email && order.email.toLowerCase().includes(input)) ||
      (order.phoneNumber && order.phoneNumber.includes(input))
    );
  });

  // عرض البيانات المفلترة
  displayOrders(filteredData);
}

// استدعاء الدالة عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", getAllOrders);
async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();
