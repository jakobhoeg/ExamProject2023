import { Loader2 } from "lucide-react";
import { ChangeEvent, FormEvent, useState } from "react";
import { toast } from "sonner";

interface RegisterFormData {
  email: string;
  password: string;
  firstName: string;
}

const RegisterForm: React.FC = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState<RegisterFormData>({
    email: "",
    password: "",
    firstName: "",
  });

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleRegister = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      setIsLoading(true);
      const response = await fetch("http://16.170.143.117:5000/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        toast.success("Successfully registered!");
        setFormData({
          email: "",
          password: "",
          firstName: "",
        });
        setIsLoading(false);
      } else {
        toast.error("Something went wrong!");
        setIsLoading(false);
      }
    } catch (error) {
      console.log(error);
    }
    setIsLoading(false);
  };

  return (
    <div className="flex flex-col gap-4 ">
      <div className="flex flex-col items-center gap-4">
        <h1 className="">Opret bruger</h1>
      </div>

      <form className="space-y-2" onSubmit={handleRegister}>
        <div className="flex flex-col gap-2">
          <label htmlFor="firstName">Fornavn</label>
          <input
            type="text"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            className="inputs "
          />
        </div>
        <div className="flex flex-col gap-2">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            className="inputs "
          />
        </div>
        <div className="flex flex-col gap-2">
          <label htmlFor="password">Kodeord</label>
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            className="inputs "
          />
        </div>
        <div className="pt-5">
          {isLoading ? (
            <button className="flex justify-center border-button w-full opacity-50">
              <Loader2 className="animate-spin h-4 w-4" />
            </button>
          ) : (
            <button
              disabled={isLoading}
              type="submit"
              className="border-button w-full disabled:opacity-50"
            >
              Opret bruger
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default RegisterForm;
