import React, {useState, useEffect} from "react";

import { accountService } from "../../_services";
import images from "../../_asset/images";

function Home() {
  const user = accountService.userValue;
  const [background, setBackground] = useState(null);

  useEffect(() => {
    images.background.then((img) => setBackground(img));
  }, []);

  return (
    <div
      className="p-4"
      style={{
        backgroundImage: `url(${background})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundRepeat: "no-repeat",
        minHeight: "100vh",
      }}
    >
      <div className="container w-100 h-100 text-white d-flex justify-center items-center">
        <div>
            <h1>Hi {user.firstName}!</h1>
            <p>You're logged in!!</p>
        </div>       
      </div>
    </div>
  );
}

export { Home };
