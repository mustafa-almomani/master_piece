var url = "https://localhost:7072/api/User/getalladmin";
let allAdmins = []; // لتخزين جميع البيانات

// دالة لجلب جميع البيانات من الـ API
async function getalladmin() {
  try {
    const response = await fetch(url);
    allAdmins = await response.json();
    renderData(allAdmins);
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

// دالة لعرض البيانات في الجدول
function renderData(data) {
  const contaner = document.getElementById("tr");
  contaner.innerHTML = ""; // تفريغ الجدول قبل إعادة العرض

  data.forEach((element) => {
    let badgeClass = element.status === "Active" ? "badge-success" : "badge-danger";

    contaner.innerHTML += `
      <tr>
        <td>
          <div class="d-flex align-items-center">
            <div class="ms-3">
              <p class="fw-bold mb-1">${element.firstName + " " + element.lastName}</p>
            </div>
          </div>
        </td>
        <td><p class="text-muted mb-0">${element.email}</p></td>
        <td>${element.phoneNumber}</td>
        <td><span class="badge ${badgeClass} rounded-pill d-inline">${element.status}</span></td>
        <td>${element.role}</td>
      </tr>`;
  });
}

// دالة لتحميل البيانات كملف PDF
function downloadPDF() {
  const { jsPDF } = window.jspdf;
  const doc = new jsPDF();

  // إعداد رأس الجدول
  const headers = [["Name", "Email", "Phone Number", "Status", "Role"]];
  
  // إعداد صفوف الجدول بناءً على البيانات
  const rows = allAdmins.map(admin => [
    `${admin.firstName} ${admin.lastName}`,
    admin.email,
    admin.phoneNumber,
    admin.status,
    admin.role
  ]);

  // إنشاء الجدول باستخدام autoTable
  doc.autoTable({
    head: headers,
    body: rows,
    startY: 20,
    theme: 'grid'
  });

  // تحميل الملف بصيغة PDF
  doc.save("Admins_Data.pdf");
}

// استدعاء الدالة عند تحميل الصفحة
getalladmin();


// دالة البحث لتصفية البيانات بناءً على المدخلات
function filterData() {
  const searchInput = document.getElementById("searchInput").value.toLowerCase();
  
  const filteredData = allAdmins.filter(
    (admin) =>
      (admin.firstName + " " + admin.lastName).toLowerCase().includes(searchInput) ||
      admin.email.toLowerCase().includes(searchInput) ||
      admin.phoneNumber.includes(searchInput)
  );

  renderData(filteredData);
}

getalladmin();


async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();
