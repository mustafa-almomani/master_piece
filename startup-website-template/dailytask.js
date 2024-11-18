let url = "https://localhost:7072/api/taskes/addtask";
let employeeMap = new Map(); // خريطة لتخزين أسماء الموظفين مع أرقامهم

async function addtaskforemp() {
  event.preventDefault();
  var form = document.getElementById("taskform");
  var formData = new FormData(form);
  var response = await fetch(url, {
    method: "POST",
    body: formData,
  });

  if (response.status == 200) {
    alert("task added successfully");
  } else {
    alert("Error in adding task");
  }
}

var taskurl = "https://localhost:7072/api/taskes/getalltaskforadmin";

async function getallorder() {
  var response = await fetch(taskurl);
  var data = await response.json();
  var contaner = document.getElementById("contaner");

  data.forEach((element) => {
    // نستخدم الخريطة لتحويل employeeId إلى اسم الموظف
    const employeeName = employeeMap.get(element.employeeId) || "Unknown"; // إذا لم يكن هناك اسم، سيتم عرض 'Unknown'

    contaner.innerHTML += `
      <tr>
        <td>${element.taskDetails}</td>
        <td>${employeeName}</td> <!-- نعرض اسم الموظف بدلاً من رقم الموظف -->
        <td>${new Date(element.assignedDate).toLocaleString()}</td>
        <td>${new Date(element.dueDate).toLocaleString()}</td>
        <td>${element.taskStatus}</td>
        <td>        <button class="btn btn-danger btn-sm" onclick="Delettask(${element.taskId})">Delete</button></td>
      </tr>
    `;
  });
}

// دالة لجلب الموظفين وتعبئة القائمة المنسدلة وإنشاء الخريطة
async function loadEmployees() {
  try {
    const response = await fetch(
      "https://localhost:7072/api/taskes/getemployees"
    );
    const employees = await response.json();
    const dropdown = document.getElementById("employeeDropdown");
    dropdown.innerHTML = '<option value="">Select Employee</option>'; // تنظيف الخيارات السابقة

    employees.forEach((employee) => {
      const option = document.createElement("option");
      option.value = employee.employeeId;
      option.textContent = employee.name;

      // إضافة الموظف إلى القائمة المنسدلة
      dropdown.appendChild(option);

      // إضافة الموظف إلى الخريطة
      employeeMap.set(employee.employeeId, employee.name);
    });
  } catch (error) {
    console.error("Error fetching employees:", error);
  }
}

// تحميل الموظفين أولاً، ثم تحميل المهام بعد ذلك
loadEmployees().then(getallorder);

async function profile() {
  employeeId = localStorage.getItem("employeeId");
  let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
  let response = await fetch(urlm);
  let data = await response.json();
  document.getElementById("nameprofile").innerHTML = data.firstName;
}

profile();




async function Delettask(id) {
  event.preventDefault();

  const result = await Swal.fire({
      title: 'Are you sure?',
      text: "Do you really want to delete this Task?",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, cancel!'
  });

  if (result.isConfirmed) {
      try {
          let url = `https://localhost:7072/api/taskes/DeleteTask/${id}`;
          let response = await fetch(url, { method: 'DELETE' });

          if (response.ok) {
              Swal.fire('Deleted!', 'Task deleted successfully', 'success');
              testimonialsData = testimonialsData.filter(t => t.id !== id);
              displayTestimonials(testimonialsData);
          } else {
              Swal.fire('Error', 'Failed to delete testimonial', 'error');
          }
      } catch (error) {
          console.error("Error:", error);
      }
  }
}
