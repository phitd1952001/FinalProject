import React from "react";
import "./style.css";

function Home() {
  return (
    <>
      <div
        id="carouselExampleCaptions"
        className="carousel slide mb-3"
        data-bs-ride="carousel"
      >
        <div className="carousel-indicators">
          <button
            type="button"
            data-bs-target="#carouselExampleCaptions"
            data-bs-slide-to="0"
            className="active"
            aria-current="true"
            aria-label="Slide 1"
          ></button>
          <button
            type="button"
            data-bs-target="#carouselExampleCaptions"
            data-bs-slide-to="1"
            aria-label="Slide 2"
          ></button>
          <button
            type="button"
            data-bs-target="#carouselExampleCaptions"
            data-bs-slide-to="2"
            aria-label="Slide 3"
          ></button>
        </div>
        <div className="carousel-inner">
          <div className="carousel-item active">
            <img
              className="img-fluid w-100 h-100 overflow-hidden d-block w-100"
              src="https://cdn.pixabay.com/photo/2016/11/14/05/15/academic-1822682_960_720.jpg"
              alt="..."
            />
            <div className="carousel-caption d-block">
              <h5>Shree Suryodaya Secondary School</h5>
              <p>
                Some representative placeholder content for the first slide.
              </p>
            </div>
          </div>
          <div className="carousel-item">
            <img
              className="img-fluid w-100 h-100 overflow-hidden d-block w-100"
              src="https://cdn.pixabay.com/photo/2017/02/05/00/08/graduation-2038864_960_720.jpg"
              alt="..."
            />
            <div className="carousel-caption d-block">
              <h5>Shree Suryodaya Secondary School</h5>
              <p>
                Some representative placeholder content for the second slide.
              </p>
            </div>
          </div>
          <div className="carousel-item">
            <img
              className="img-fluid w-100 h-100 overflow-hidden d-block w-100"
              src="https://cdn.pixabay.com/photo/2017/09/08/00/37/friend-2727305_960_720.jpg"
              alt="..."
            />
            <div className="carousel-caption d-block">
              <h5>Shree Suryodaya Secondary School</h5>
              <p>
                Some representative placeholder content for the third slide.
              </p>
            </div>
          </div>
        </div>
        <button
          className="carousel-control-prev"
          type="button"
          data-bs-target="#carouselExampleCaptions"
          data-bs-slide="prev"
        >
          <span
            className="carousel-control-prev-icon"
            aria-hidden="true"
          ></span>
          <span className="visually-hidden">Previous</span>
        </button>
        <button
          className="carousel-control-next"
          type="button"
          data-bs-target="#carouselExampleCaptions"
          data-bs-slide="next"
        >
          <span
            className="carousel-control-next-icon"
            aria-hidden="true"
          ></span>
          <span className="visually-hidden">Next</span>
        </button>
      </div>

      <main id="main">
        <section id="about" className="about mt-5">
          <div className="container-fluid">
            <h2 className="h1-responsive font-weight-bold text-center my-2">
              Introduction to Greenwich University Vietnam
            </h2>

            <p className="text-center w-responsive mx-auto mb-1">
              Welcome to Greenwich University Vietnam! If you have any
              questions, feel free to contact us directly. Our team will get
              back to you within a matter of hours to assist you.
            </p>
            <div className="row  pt-5 pb-5">
              <div
                className="col-lg-5 align-items-stretch video-box"
                style={{
                  backgroundImage:
                    'url("https://res.cloudinary.com/dafhoj5q5/image/upload/v1699690731/sphs6nsv5t6uatmtumx5.jpg")',
                }}
              >
                <a
                  href="https://www.youtube.com/watch?v=hyTkx3y4AHY"
                  className="venobox play-btn mb-4"
                  data-vbtype="video"
                  data-autoplay="true"
                ></a>
              </div>

              <div className="col-lg-7 d-flex flex-column justify-content-center align-items-stretch">
                <div className="content">
                  <h3>
                    About Greenwich University Vietnam{" "}
                    <strong>Empowering Minds, Enriching Futures</strong>
                  </h3>
                  <p>
                    Welcome to Greenwich University Vietnam, where we take pride
                    in offering a diverse and high-quality learning experience.
                    Our commitment is to create a positive learning environment
                    and support the comprehensive development of our students.
                  </p>
                  <p className="font-italic">
                    Our vision is to be a premier university, producing
                    high-quality graduates who make a positive impact on the
                    community and society. Our mission is to provide quality
                    education, promote research, and foster the personal
                    development of every student.
                  </p>
                  <ul>
                    <li>
                      <i className="bx bx-check-double"></i> Dedicated to
                      delivering top-notch education through an experienced
                      faculty and a diverse curriculum.
                    </li>
                    <li>
                      <i className="bx bx-check-double"></i> Students not only
                      learn in modern facilities but also have opportunities to
                      engage in extracurricular activities and interesting
                      research projects.
                    </li>
                    <li>
                      <i className="bx bx-check-double"></i> If you have any
                      questions or seek more information, reach out to us
                      anytime. We're here to assist you with your educational
                      and personal development queries.
                    </li>
                  </ul>
                  <p>
                    Thank you for considering Greenwich University Vietnam. We
                    eagerly anticipate the opportunity to welcome you into our
                    academic community!
                  </p>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section>
          <div className="container course pb-5 pt-5">
            <h2 className="h1-responsive font-weight-bold text-center my-4">
              Courses
            </h2>

            <p className="text-center w-responsive mx-auto mb-5">
              Do you have any questions? Please do not hesitate to contact us
              directly. Our team will come back to you within a matter of hours
              to help you.
            </p>
            <div className="row">
              <div className="col-md-4">
                <div className="card box">
                  <div
                    className="bg-image hover-overlay ripple"
                    data-mdb-ripple-color="light"
                  >
                    <img
                      src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1699691764/j14j6ezslnsshsiexaoy.png"
                      className="img-fluid"
                    />
                    <a href="#!">
                      <div
                        className="mask"
                        style={{ backgroundColor: "rgba(251, 251, 251, 0.15)" }}
                      ></div>
                    </a>
                  </div>
                  <div className="card-body">
                    <h5 className="card-title font-weight-bold">
                      Information Technology
                    </h5>
                    <p className="card-text text-muted">
                      Dive into the world of Information Technology with our
                      comprehensive course. Acquire practical skills in
                      programming, data management, and network administration.
                    </p>
                    <a href="#!" className="btn btn-primary">
                      Read More
                    </a>
                  </div>
                </div>
              </div>

              <div className="col-md-4">
                <div className="card box">
                  <div
                    className="bg-image hover-overlay ripple"
                    data-mdb-ripple-color="light"
                  >
                    <img
                      src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1699692028/l7tisfb0qllp9l6owbzx.jpg"
                      className="img-fluid"
                    />
                    <a href="#!">
                      <div
                        className="mask"
                        style={{ backgroundColor: "rgba(251, 251, 251, 0.15)" }}
                      ></div>
                    </a>
                  </div>
                  <div className="card-body">
                    <h5 className="card-title font-weight-bold">
                      Business Administration
                    </h5>
                    <p className="card-text text-muted">
                      Develop strong managerial skills and business acumen in
                      our Business Administration course. Learn strategic
                      decision-making, financial analysis, and effective
                      leadership.
                    </p>
                    <a href="#!" className="btn btn-primary">
                      Read More
                    </a>
                  </div>
                </div>
              </div>

              <div className="col-md-4">
                <div className="card box">
                  <div
                    className="bg-image hover-overlay ripple"
                    data-mdb-ripple-color="light"
                  >
                    <img
                      src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1699692270/gn7xalgxgnyi7espmvkf.webp"
                      className="img-fluid"
                    />
                    <a href="#!">
                      <div
                        className="mask"
                        style={{ backgroundColor: "rgba(251, 251, 251, 0.15)" }}
                      ></div>
                    </a>
                  </div>
                  <div className="card-body">
                    <h5 className="card-title font-weight-bold">
                      Event Management
                    </h5>
                    <p className="card-text text-muted">
                      Dive into the exciting field of Event Management. From
                      planning to execution, our course covers all aspects of
                      organizing successful events, conferences, and
                      exhibitions.
                    </p>
                    <a href="#!" className="btn btn-primary">
                      Read More
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section>
          <div className="container">
            <h2 className="h1-responsive font-weight-bold text-center my-4">
              Gallery
            </h2>

            <p className="text-center w-responsive mx-auto mb-5">
              Do you have any questions? Please do not hesitate to contact us
              directly. Our team will come back to you within a matter of hours
              to help you.
            </p>

            <div className="row">
              <div className="col-lg-4 col-md-12 mb-4 mb-lg-0">
                <img
                  src="https://cdn.pixabay.com/photo/2016/06/25/12/52/laptop-1478822_960_720.jpg"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Boat on Calm Water"
                />

                <img
                  src="https://mdbcdn.b-cdn.net/img/Photos/Vertical/mountain1.webp"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Wintry Mountain Landscape"
                />
              </div>

              <div className="col-lg-4 mb-4 mb-lg-0">
                <img
                  src="https://mdbcdn.b-cdn.net/img/Photos/Vertical/mountain2.webp"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Mountains in the Clouds"
                />

                <img
                  src="https://mdbcdn.b-cdn.net/img/Photos/Horizontal/Nature/4-col/img%20(73).webp"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Boat on Calm Water"
                />
              </div>

              <div className="col-lg-4 mb-4 mb-lg-0">
                <img
                  src="https://mdbcdn.b-cdn.net/img/Photos/Horizontal/Nature/4-col/img%20(18).webp"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Waves at Sea"
                />

                <img
                  src="https://mdbcdn.b-cdn.net/img/Photos/Vertical/mountain3.webp"
                  className="w-100 shadow-1-strong rounded mb-4"
                  alt="Yosemite National Park"
                />
              </div>
            </div>
          </div>
        </section>

        <section>
          <div className="container mb-5">
            <section className="mb-4">
              <h2 className="h1-responsive font-weight-bold text-center my-4">
                Contact us
              </h2>

              <p className="text-center w-responsive mx-auto mb-5">
                Do you have any questions? Please do not hesitate to contact us
                directly. Our team will come back to you within a matter of
                hours to help you.
              </p>

              <div className="row">
                <div className="col-md-6 mb-md-0 mb-5">
                  <form
                    id="contact-form"
                    name="contact-form"
                    action="mail.php"
                    method="POST"
                  >
                    <div className="row">
                      <div className="col-md-6">
                        <div className="md-form mb-0">
                          <input
                            type="text"
                            id="name"
                            name="name"
                            className="form-control"
                          />
                          <label className="">Your name</label>
                        </div>
                      </div>

                      <div className="col-md-6">
                        <div className="md-form mb-0">
                          <input
                            type="text"
                            id="email"
                            name="email"
                            className="form-control"
                          />
                          <label className="">Your email</label>
                        </div>
                      </div>
                    </div>

                    <div className="row">
                      <div className="col-md-12">
                        <div className="md-form mb-0">
                          <input
                            type="text"
                            id="subject"
                            name="subject"
                            className="form-control"
                          />
                          <label className="">Subject</label>
                        </div>
                      </div>
                    </div>

                    <div className="row">
                      <div className="col-md-12">
                        <div className="md-form">
                          <textarea
                            type="text"
                            id="message"
                            name="message"
                            rows="2"
                            className="form-control md-textarea"
                          ></textarea>
                          <label>Your message</label>
                        </div>
                      </div>
                    </div>
                  </form>

                  <div className="text-center text-md-left">
                    <a className="btn btn-primary">Send</a>
                  </div>
                  <div className="status"></div>
                </div>

                <div className="col-md-6 text-center">
                  <iframe
                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.105431984496!2d106.65025617481858!3d10.803236458699073!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752e639bf01243%3A0x700ebdccb5a04987!2zVHLGsOG7nW5nIMSQ4bqhaSBo4buNYyBHcmVlbndpY2ggVknhu4ZUIE5BTQ!5e0!3m2!1svi!2s!4v1699675515464!5m2!1svi!2s"
                    width="600"
                    height="300px"
                    style={{ border: 0 }}
                    allowFullScreen={true}
                    loading="lazy"
                    referrerPolicy="no-referrer-when-downgrade"
                  ></iframe>
                </div>
              </div>
            </section>
          </div>
        </section>

        <footer className="text-center text-lg-start bg-dark py-3 text-white">
          <section className="">
            <div className="container text-center text-md-start mt-5">
              <div className="row mt-3">
                <div className="col-md-3 col-lg-4 col-xl-3 mx-auto mb-4">
                  <h6 className="text-uppercase fw-bold mb-4">
                    <i className="bi bi-gem me-3"></i>Company name
                  </h6>
                  <p>
                    Here you can use rows and columns to organize your footer
                    content. Lorem ipsum dolor sit amet, consectetur adipisicing
                    elit.
                  </p>
                </div>

                <div className="col-md-2 col-lg-2 col-xl-2 mx-auto mb-4">
                  <h6 className="text-uppercase fw-bold mb-4">Products</h6>
                  <p>
                    <a href="#!" className="text-reset">
                      Angular
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      React
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      Vue
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      Laravel
                    </a>
                  </p>
                </div>

                <div className="col-md-3 col-lg-2 col-xl-2 mx-auto mb-4">
                  <h6 className="text-uppercase fw-bold mb-4">Useful links</h6>
                  <p>
                    <a href="#!" className="text-reset">
                      Pricing
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      Settings
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      Orders
                    </a>
                  </p>
                  <p>
                    <a href="#!" className="text-reset">
                      Help
                    </a>
                  </p>
                </div>

                <div className="col-md-4 col-lg-3 col-xl-3 mx-auto mb-md-0 mb-4">
                  <h6 className="text-uppercase fw-bold mb-4">Contact</h6>
                  <p>
                    <i className="bi bi-location me-3"></i> New York, NY 10012,
                    US
                  </p>
                  <p>
                    <i className="bi bi-envelope me-3"></i>
                    info@example.com
                  </p>
                  <p>
                    <i className="bi bi-phone me-3"></i> + 01 234 567 88
                  </p>
                  <p>
                    <i className="bi bi-print me-3"></i> + 01 234 567 89
                  </p>
                </div>
              </div>
            </div>
          </section>

          <div
            className="text-center py-4"
            style={{ backgroundColor: "#08090a" }}
          >
            © 2021 Copyright:
            <a
              className="text-reset fw-bold"
              href="https://tuyensinh.greenwich.edu.vn/"
            >
              greenwichUniversity.com
            </a>
          </div>
        </footer>
      </main>
    </>
  );
}

export { Home };
