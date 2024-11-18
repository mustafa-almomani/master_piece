// Creates a new testimonial
async function Addtestimonial() {
  event.preventDefault();
  let user = localStorage.getItem("UserID");
  if (user == null || user == undefined) {
    await Swal.fire({
      icon: "warning",
      title: "Login Required",
      text: "You need to be logged in to submit a testimonial.",
    });
    return;
  }

  let message = document.getElementById("testimonialtext").value;
  let url = `https://localhost:7072/api/User/AddTestimonial/${user}`;
  let data = { theTestimonials: message };

  let response = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (response.ok) {
    await Swal.fire({
      icon: "success",
      title: "Success!",
      text: "Testimonial submitted successfully. We are happy to see your testimonial.",
    });
    document.getElementById("testimonialtext").value = "";
  } else {
    await Swal.fire({
      icon: "error",
      title: "Submission Failed",
      text: "Failed to submit the testimonial.",
    });
  }
}

async function gettestimonial() {
  let testimonials = document.getElementById("testimonials");
  let url = "https://localhost:7072/api/User/GetAllAcceptedTestimonial";

  try {
    let response = await fetch(url);
    let data = await response.json();

    // مسح المحتوى القديم إذا لزم الأمر
    testimonials.innerHTML = "";
    data.forEach((item) => {
      testimonials.innerHTML += `
        <div class="testimonial-item bg-light my-4 mx-2 p-4 rounded shadow">
          <div class="d-flex align-items-center border-bottom pt-3 pb-3">
            <img class="img-fluid rounded-circle me-3" 
              src="https://static.vecteezy.com/system/resources/thumbnails/009/292/244/small/default-avatar-icon-of-social-media-user-vector.jpg" 
              style="width: 60px; height: 60px;">
            <div>
              <h5 class="text-primary mb-1">${item.username}</h5>
              <small class="text-muted">${item.email}</small>
            </div>
          </div>
          <div class="pt-3">
            <p class="mb-0">${item.theTestimonials}</p>
          </div>
        </div>
      `;
    });

    // إعادة تهيئة السلايدر بعد إضافة العناصر
    $(".testimonial-carousel").owlCarousel("destroy");
    initializeOwlCarousel();
  } catch (error) {
    console.error("Error fetching testimonials:", error);
  }
}

function initializeOwlCarousel() {
  $(".testimonial-carousel").owlCarousel({
    autoplay: true,
    smartSpeed: 1000,
    center: true,
    dots: false,
    loop: true,
    nav: true,
    navText: [
      '<i class="bi bi-arrow-left-circle-fill"></i>',
      '<i class="bi bi-arrow-right-circle-fill"></i>',
    ],
    responsive: {
      0: {
        items: 1,
      },
      768: {
        items: 2,
      },
      1024: {
        items: 3,
      },
    },
  });
}

gettestimonial();
