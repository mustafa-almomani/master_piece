let testimonialsData = []; // مصفوفة لتخزين جميع الشهادات

// دالة لجلب جميع الشهادات من الـ API
async function AllTestimonials() {
    try {
        let url = "https://localhost:7072/api/User/GetAllTestimonials";
        let response = await fetch(url);
        if (response.ok) {
            testimonialsData = await response.json();
            displayTestimonials(testimonialsData);
        } else {
            console.error("Failed to fetch testimonials");
        }
    } catch (error) {
        console.error("Error:", error);
    }
}

// دالة لعرض الشهادات بناءً على البيانات الممررة
function displayTestimonials(data) {
    let allTestimonials = document.getElementById("testimonialid");
    allTestimonials.innerHTML = '';

    data.forEach(element => {
        let row = `
        <tr>
            <td>
                <div class="d-flex px-2">
                    <div class="my-auto">
                        <h6 class="mb-0 text-sm ms-2">${element.username}</h6>
                    </div>
                </div>
            </td>
            <td><p class="text-sm font-weight-bold mb-0">${element.email}</p></td>
            <td class="text-wrap" style="min-width: 200px; max-width: 650px;">
                <span class="text-xs font-weight-bold">${element.theTestimonials}</span>
            </td>
            <td class="align-middle">
                ${element.isaccepted ? `
                    <button class="btn btn-danger btn-sm" onclick="DeletTestimonial(${element.id})">Delete</button>
                ` : `
                    <button class="btn btn-warning btn-sm" onclick="AcceptTestimonial(${element.id})">Accept</button>
                    <button class="btn btn-danger btn-sm" onclick="DeletTestimonial(${element.id})">Delete</button>
                `}
            </td>
        </tr>`;
        allTestimonials.innerHTML += row;
    });
}

// دالة الفلترة
function filterTestimonials(filter) {
    if (filter === 'all') {
        displayTestimonials(testimonialsData);
    } else if (filter === 'accepted') {
        const acceptedTestimonials = testimonialsData.filter(t => t.isaccepted);
        displayTestimonials(acceptedTestimonials);
    } else if (filter === 'pending') {
        const pendingTestimonials = testimonialsData.filter(t => !t.isaccepted);
        displayTestimonials(pendingTestimonials);
    }
}

// دالة لحذف شهادة
async function DeletTestimonial(id) {
    event.preventDefault();

    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "Do you really want to delete this testimonial?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!'
    });

    if (result.isConfirmed) {
        try {
            let url = `https://localhost:7072/api/User/DeleteTestimonial/${id}`;
            let response = await fetch(url, { method: 'DELETE' });

            if (response.ok) {
                Swal.fire('Deleted!', 'Testimonial deleted successfully', 'success');
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

// دالة لقبول شهادة
async function AcceptTestimonial(id) {
    event.preventDefault();

    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "Do you really want to accept this testimonial?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, accept it!',
        cancelButtonText: 'No, cancel!'
    });

    if (result.isConfirmed) {
        try {
            let url = `https://localhost:7072/api/User/AcceptTestimonial/${id}`;
            let response = await fetch(url, { method: 'PUT' });

            if (response.ok) {
                Swal.fire('Accepted!', 'Testimonial accepted successfully', 'success');
                testimonialsData = testimonialsData.map(t => {
                    if (t.id === id) t.isaccepted = true;
                    return t;
                });
                displayTestimonials(testimonialsData);
            } else {
                Swal.fire('Error', 'Failed to accept testimonial', 'error');
            }
        } catch (error) {
            console.error("Error:", error);
        }
    }
}

// استدعاء الدالة عند تحميل الصفحة
AllTestimonials();



async function profile() {
    employeeId = localStorage.getItem("employeeId");
    let urlm = `https://localhost:7072/api/User/getemployeebyid/${employeeId}`;
    let response = await fetch(urlm);
    let data = await response.json();
    document.getElementById("nameprofile").innerHTML = data.firstName;
  }
  
  profile();