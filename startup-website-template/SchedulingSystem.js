const url = "https://localhost:7072/api/ScheduledMeeting/adminRespone";

async function clientForm(event) {
  event.preventDefault(); // منع إرسال النموذج بشكل افتراضي
  // احصل على النموذج
  var form = document.getElementById("clientFormm");
  var formData = new FormData(form);

  // احصل على القائمة المنسدلة
  var meetingPlatformSelect = document.getElementById("meetingPlatform");
  var selectedOption =
    meetingPlatformSelect.options[meetingPlatformSelect.selectedIndex];

  // احصل على القيم من القائمة المنسدلة
  var meetingPlatform = selectedOption.value; // هذا هو اسم المنصة
  var meetingLink = selectedOption.getAttribute("data-link"); // هذا هو الرابط من الخاصية data-link

  // أضف القيم إلى formData
  formData.set("MeetingLink", meetingLink); // الرابط
  formData.set("MeetingPlatform", meetingPlatform); // اسم المنصة

  console.log("MeetingLink:", meetingLink); // التأكيد على القيم المخزنة
  console.log("MeetingPlatform:", meetingPlatform);

  var response = await fetch(url, {
    method: "POST",
    body: formData,
  });

  if (response.ok) {
    Swal.fire({
      title: "Success!",
      text: "Email sent successfully",
      icon: "success",
      confirmButtonText: "OK",
      timer: 3000, // الوقت بالملي ثانية
      timerProgressBar: true, // إظهار شريط تقدم للوقت
    });

    // إعادة التوجيه بعد 2 ثانية
    setTimeout(() => {
      window.location.href = "SchedulingSystem.html";
    }, 2000);
  } else {
    Swal.fire({
      title: "Error!",
      text: "Email sent failed",
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}

async function loadEmployees() {
  try {
    const response = await fetch(
      "https://localhost:7072/api/meeting/getemployees"
    );
    const employees = await response.json();

    const employeeSelect = document.getElementById("employeeSelect");
    employeeSelect.innerHTML = ""; // تنظيف الخيارات السابقة

    // تعبئة القائمة المنسدلة بأسماء الموظفين
    employees.forEach((employee) => {
      const option = document.createElement("option");
      option.value = employee.employeeId; // المعرف
      option.textContent = employee.fullName; // الاسم
      employeeSelect.appendChild(option);
    });
  } catch (error) {
    console.error("Error fetching employees:", error);
  }
}

// استدعاء دالة جلب الموظفين عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", loadEmployees);

const url2 = "https://localhost:7072/api/ScheduledMeeting/employeemeeting";
async function teamForm() {
  event.preventDefault(); // منع التحديث التلقائي للصفحة

  var form = document.getElementById("teamForm");
  var formData = new FormData(form);

  // حذف أي مدخلات سابقة للـ EmployeeId لتجنب التكرار
  formData.delete("EmployeeId");

  // الحصول على قائمة المعرفات المختارة للموظفين
  var selectedEmployees = Array.from(
    document.getElementById("employeeSelect").selectedOptions
  ).map((option) => option.value); // تحويلها إلى مصفوفة معرفات

  // إضافة معرفات الموظفين إلى formData مرة واحدة فقط
  selectedEmployees.forEach((employeeId) => {
    formData.append("EmployeeId", employeeId);
  });

  // إرسال الطلب إلى الـ API
  var response = await fetch(url2, {
    method: "POST",
    body: formData,
  });

  if (response.ok) {
    Swal.fire({
      title: "Success!",
      text: "Meeting scheduled successfully",
      icon: "success",
      confirmButtonText: "OK",
      timer: 3000,
      timerProgressBar: true,
    });

    setTimeout(() => {}, 2000);
  } else {
    Swal.fire({
      title: "Error!",
      text: "Failed to schedule meeting",
      icon: "error",
      confirmButtonText: "OK",
    });
  }
}

var allMeetings = {
  employeeMeetings: [],
  customerMeetings: [],
};

async function getAllMeetings() {
  const url = "https://localhost:7072/api/meeting/getallmeeting";
  try {
    const response = await fetch(url);
    const data = await response.json();
    
    // حفظ الاجتماعات في المتغير العام لاستخدامه في البحث
    allMeetings.employeeMeetings = data.employeeMeetings;
    allMeetings.customerMeetings = data.customerMeetings;

    // عرض الاجتماعات عند التحميل
    renderMeetings(allMeetings);
  } catch (error) {
    console.error("Error fetching meetings:", error);
  }
}

// دالة لعرض الاجتماعات في الجدول
function renderMeetings(meetings) {
  const employeeMeetingsTableBody = document.getElementById("employeeMeetingsTableBody");
  const customerMeetingsTableBody = document.getElementById("customerMeetingsTableBody");

  // تفريغ الجداول قبل إعادة العرض
  employeeMeetingsTableBody.innerHTML = "";
  customerMeetingsTableBody.innerHTML = "";

  // عرض اجتماعات الموظفين
  meetings.employeeMeetings.forEach((meeting) => {
    employeeMeetingsTableBody.innerHTML += `
      <tr data-datetime="${new Date(meeting.meetingDateTime).toISOString()}">
          <td>Team Meeting</td>
          <td>${new Date(meeting.meetingDateTime).toLocaleString()}</td>
          <td>${meeting.employees}</td>
          <td>${meeting.meetingAgenda}</td>
          <td><a href="${meeting.linkmeeting}" target="_blank">Zoom</a></td>
          <td>
            <button class="btn btn-danger btn-sm" onclick="deleteMeetingsByDateTime('${new Date(meeting.meetingDateTime).toISOString()}')">Cancel</button>
          </td>
      </tr>`;
  });

  // عرض اجتماعات العملاء
  meetings.customerMeetings.forEach((meeting) => {
    customerMeetingsTableBody.innerHTML += `
      <tr data-datetime="${new Date(meeting.meetingDateTime).toISOString()}">
          <td>Client Meeting</td>
          <td>${new Date(meeting.meetingDateTime).toLocaleString()}</td>
          <td>${meeting.customerId}</td>
          <td>${meeting.meetingAgenda}</td>
          <td><a href="${meeting.meetinglink}" target="_blank">Zoom</a></td>
          <td>
            <button class="btn btn-danger btn-sm" onclick="deleteMeetingsByDateTime('${new Date(meeting.meetingDateTime).toISOString()}')">Cancel</button>
          </td>
      </tr>`;
  });
}

// دالة لتصفية الاجتماعات بناءً على المدخلات
function filterMeetings() {
  const searchInput = document.getElementById("searchInput").value.toLowerCase();

  // تصفية اجتماعات الموظفين والعملاء
  const filteredEmployeeMeetings = allMeetings.employeeMeetings.filter(
    (meeting) =>
      meeting.employees.toLowerCase().includes(searchInput) ||
      meeting.meetingAgenda.toLowerCase().includes(searchInput)
  );

  const filteredCustomerMeetings = allMeetings.customerMeetings.filter(
    (meeting) =>
      meeting.customerId.toLowerCase().includes(searchInput) ||
      meeting.meetingAgenda.toLowerCase().includes(searchInput)
  );

  // إعادة عرض الاجتماعات بناءً على الفلتر
  renderMeetings({
    employeeMeetings: filteredEmployeeMeetings,
    customerMeetings: filteredCustomerMeetings,
  });
}

// استدعاء دالة جلب الاجتماعات عند تحميل الصفحة
getAllMeetings();

// استدعاء الدالة لجلب البيانات عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", getAllMeetings);

async function deleteMeetingsByDateTime(meetingDateTime) {
  // استخدام SweetAlert للتأكيد على الحذف
  Swal.fire({
    title: "Are you sure?",
    text: "Do you want to delete all meetings at this time?",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then(async (result) => {
    if (result.isConfirmed) {
      // إذا تم التأكيد من المستخدم
      const formattedDateTime = new Date(meetingDateTime).toISOString();

      // رابط الـ API للحذف
      const url = `https://localhost:7072/api/meeting/deletemeetbydatetime/${formattedDateTime}`;

      try {
        const response = await fetch(url, {
          method: "DELETE",
        });

        // تحقق من نجاح الحذف
        if (response.ok) {
          Swal.fire("Deleted!", "Meetings have been deleted.", "success");

          // حذف الصفوف التي تحتوي على هذا التاريخ من الجدول
          document
            .querySelectorAll(`tr[data-datetime="${formattedDateTime}"]`)
            .forEach((row) => row.remove());
        } else {
          Swal.fire("Error!", "Error deleting meetings", "error");
          console.error("Error:", await response.text());
        }
      } catch (error) {
        console.error("Error fetching API:", error);
        Swal.fire("Error!", "Error deleting meetings", "error");
      }
    }
  });
}

function getUserDataForMeeting() {
  const fullName = localStorage.getItem("meetingFullName");
  const phone = localStorage.getItem("meetingPhone");
  const email = localStorage.getItem("meetingEmail");
  const meetingdate = localStorage.getItem("meetingEmail");


  if (fullName && phone && email) {
    // تعيين الاسم الكامل في حقل الاسم الكامل
    document.getElementById("clientFullName").value = fullName;

    // تعيين رقم الهاتف والإيميل
    document.getElementById("clientPhone").value = phone;
    document.getElementById("clientEmail").value = email;
    document.getElementById("meetingDate").value = meetingDate;


    // بعد تعيين القيم، قم بحذف البيانات من localStorage
    localStorage.removeItem("meetingFullName");
    localStorage.removeItem("meetingPhone");
    localStorage.removeItem("meetingEmail");
  }
}

// استدعاء الدالة عند تحميل الصفحة
getUserDataForMeeting();

async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();
