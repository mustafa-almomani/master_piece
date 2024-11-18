var orderurl = "https://localhost:7072/api/services/getallorder";
var allOrders = []; // لتخزين جميع الطلبات بشكل عام

async function getallorder() {
  var response = await fetch(orderurl);
  var data = await response.json();
  allOrders = data; // تخزين جميع الطلبات في المتغير العام

  displayOrders(allOrders); // عرض جميع الطلبات عند التحميل الأول
}

// دالة لعرض الطلبات في حاوية contaner
function displayOrders(orders) {
  var contaner = document.getElementById("contaner");
  contaner.innerHTML = ""; // مسح المحتوى الحالي

  orders.forEach((element) => {
    contaner.innerHTML += `
            <div class="col-xl-3 col-md-6 mb-4">
                <div class="card border-primary shadow-sm h-100">
                    <div class="card-header bg-primary text-white d-flex justify-content-between">
                        <h5 class="mb-0">Order ${element.orderNumber}</h5>
                        <i class="fas fa-folder-open icon" onClick="openModal('${
                          element.userid
                        }','${element.requestid}','${element.email}')"></i>
                    </div>
                    <div class="card-body">
                        <div class="order-info">
                            <p class="text-truncate"><i class="fas fa-user"></i> <strong>Name:</strong> ${
                              element.firstname + " " + element.lastname
                            }</p>
                            <p class="text-truncate"><i class="fas fa-building"></i> <strong>Company:</strong> ${
                              element.companyname
                            }</p>
                            <p class="text-truncate"><i class="fas fa-phone"></i> <strong>Phone:</strong> ${
                              element.phonnumber
                            }</p>
                            <p class="text-truncate"><i class="fas fa-envelope"></i> <strong>Email:</strong> ${
                              element.email
                            }</p>
                            <p class="text-truncate" id="mm"><i class="fas fa-laptop-code"></i> <strong>Project:</strong> ${
                              element.servicename
                            }</p>
                        </div>

                        <!-- دروب داون ليست للحالة -->
                        <div class="dropdown mt-3">
                            <select onchange="editstatus(${element.requestid})" id="status-${ element.requestid }" class="form-select form-select-sm text-uppercase fw-bold bg-light border-0 shadow-sm">
                                <option value="" class="text-Info">${
                                  element.status
                                }</option>
                                <option value="Pending" class="text-Info">Pending</option>
                                <option value="In Progress" class="text-primary">In Progress</option>
                                <option value="Completed" class="text-success">Completed</option>
                            </select>
                        </div>
                    </div>

                    <button type="button" onClick="batool('${
                      element.servicename
                    }', ${
      element.orderNumber
    })" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal" data-bs-target="#orderModal-${
      element.orderNumber
    }">
                        View Details
                    </button>
                    <a href="SchedulingSystem.html" 
                       class="btn btn-outline-primary btn-sm"
                       onClick="storeUserDataForMeeting('${
                         element.firstname
                       }', '${element.lastname}', '${element.phonnumber}', '${element.email
}')">
                        Schedule Meeting
                    </a>
                </div>
            </div>

            <!-- Modal لعرض التفاصيل -->
            <div class="modal fade" id="orderModal-${
              element.orderNumber
            }" tabindex="-1" aria-labelledby="orderModalLabel-${
      element.orderNumber
    }" aria-hidden="true">
              <div class="modal-dialog">
                <div class="modal-content">
                  <div class="modal-header">
                    <h5 class="modal-title" id="orderModalLabel-${
                      element.orderNumber
                    }">Order Details - ${element.orderNumber}</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                  </div>
                  <div class="modal-body text-dark">
                    <p style="display: none;" id="ddd-${element.orderNumber}">${
      element.requestid
    }</p>
                    <p><strong>Name:</strong> ${
                      element.firstname + " " + element.lastname
                    }</p>
                    <p><strong>Company:</strong> ${element.companyname}</p>
                    <p><strong>Phone:</strong> ${element.phonnumber}</p>
                    <p><strong>Email:</strong> ${element.email}</p>
                    <p><strong>Project:</strong> ${element.servicename}</p>
                    <p><strong>Project Details:</strong><p id="projectdetails-${
                      element.orderNumber
                    }">${element.projectdetails}</p></p>
                    <p><strong>Request Date:</strong> ${new Date(
                      element.requestdate
                    ).toLocaleString()}</p>
                      <p><strong>Meeting Date:</strong> ${new Date(
                      element.meetingdate
                    ).toLocaleString()}</p>
<img src="https://localhost:7072/Uploads/${element.img}" width="150px" alt="">
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                  </div>
                </div>
              </div>
            </div>
        `;
  });
}

// دالة لفلترة الطلبات حسب الحالة
function filterOrdersByStatus(status) {
  if (status === "all") {
    displayOrders(allOrders); // عرض جميع الطلبات
  } else {
    var filteredOrders = allOrders.filter((order) => order.status === status);
    displayOrders(filteredOrders); // عرض الطلبات حسب الحالة المحددة
  }
}

// استدعاء دالة getallorder عند تحميل الصفحة
getallorder();

// دالة لتحويل بيانات الطلب إلى ملف نصي وتحميله
// function downloadOrderInfo(orderNumber, firstname, lastname, companyname, email, servicename, projectdetails) {
//   const orderInfo = `
//         Order Number: ${orderNumber}
//         Name: ${firstname} ${lastname}
//         Company: ${companyname}
//         Email: ${email}
//         Service Name: ${servicename}
//         Project Details: ${projectdetails}
//     `;

//   const blob = new Blob([orderInfo], { type: "text/plain" });
//   const a = document.createElement("a");
//   const url = window.URL.createObjectURL(blob);
//   a.href = url;
//   a.download = `Order_${orderNumber}.txt`;
//   document.body.appendChild(a);
//   a.click();
//   window.URL.revokeObjectURL(url);
//   document.body.removeChild(a);
// }

getallorder();

// دالة لجلب قائمة الموظفين
async function getEmployees(servicename, orderNumber) {
  if (!servicename) {
    alert("Please enter a service name.");
    return;
  }

  try {
    const response = await fetch(
      `https://localhost:7072/api/services/getjoptitleforemployee?serviceName=${servicename}`
    );

    if (!response.ok) {
      throw new Error("Failed to fetch employees");
    }

    const data = await response.json();
    const dropdown = document.getElementById(`employeeDropdown-${orderNumber}`);
    dropdown.innerHTML = ""; // مسح أي نتائج سابقة

    dropdown.innerHTML = '<option value="">Select an employee</option>';

    if (data.length === 0) {
      dropdown.innerHTML = '<option value="">No employees found</option>';
      return;
    }

    data.forEach((employee) => {
      // يتم استخدام employeeId كـ value وemployeeName كالنص الظاهر
      dropdown.innerHTML += `
        <option value="${employee.employeeid}">${employee.employeeName} - ${employee.jobTitle}</option>
      `;
    });
  } catch (error) {
    console.error("Error fetching data:", error);
    alert("An error occurred while fetching employees.");
  }
}

async function editstatus(id) {

  event.preventDefault();
  debugger;
  let urlm = `https://localhost:7072/api/CustomerManagement/editorder/${id}`;
  let newStatus = document.getElementById(`status-${id}`).value;

  let response = await fetch(urlm, {
    method: "PUT",
    body: JSON.stringify({
      status: newStatus,
    }),
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (response.status == 200) {
    alert("Status updated successfully");

    // تحديث الحالة في allOrders وتحديث الواجهة
    let order = allOrders.find((order) => order.requestid === id);
    if (order) {
      order.status = newStatus;
    }

    // عرض الطلبات من جديد بناءً على الحالة الجديدة
    displayOrders(allOrders);
  } else {
    alert("Error updating status");
  }
}

function storeUserDataForMeeting(firstname, lastname, phone, email) {
  // جمع الاسم الأول والاسم الأخير
  const fullName = firstname + " " + lastname;

  // تخزين الاسم الكامل، رقم الهاتف، والإيميل في localStorage
  localStorage.setItem("meetingFullName", fullName);
  localStorage.setItem("meetingPhone", phone);
  localStorage.setItem("meetingEmail", email);
}

async function profile() {
  debugger;
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();

function openModal(userid, orderid, email) {
  debugger;
  var myModal = new bootstrap.Modal(document.getElementById("myModal"));
  document.getElementById("orderId").value = orderid;
  document.getElementById("userId").value = userid;
  document.getElementById("email").value = email;

  myModal.show();
}
async function sendPaymentLink() {
  event.preventDefault();
  // جمع بيانات النموذج
  const email = document.getElementById("email").value;
  const userId = document.getElementById("userId").value;
  const orderId = document.getElementById("orderId").value;
  const amount = document.getElementById("amount").value;
  const contract = document.getElementById("contract").files[0]; // الحصول على الملف

  // التحقق من اختيار الملف
  if (!contract) {
    window.alert("يرجى اختيار ملف العقد.");
    return;
  }

  // إعداد FormData لإرسال البيانات مع الملف
  const formData = new FormData();
  formData.append("Email", email);
  formData.append("UserId", userId);
  formData.append("OredrID", orderId);
  formData.append("amount", amount);
  formData.append("pdfFile", contract);

  try {
    // Send a POST request to the API
    const response = await fetch(
      "https://localhost:7072/api/Payment/adminResponse",
      {
        method: "POST",
        body: formData,
      }
    );

    if (response.ok) {
      const result = await response.text();
      alert("Success: " + result);
      window.location.reload();
    } else {
      const errorText = await response.text();
      alert("Error: " + errorText);
    }
  } catch (error) {
    console.error("Error:", error);
    alert("Email sending  Successfully");
  }
}
