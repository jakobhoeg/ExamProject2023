// Login.tsx
import React, { ChangeEvent, FormEvent, useState } from "react";

import { toast } from "sonner";
import { useAuth } from "../context/AuthProvider";
import { Loader2 } from "lucide-react";

interface LoginFormData {
  email: string;
  password: string;
}

const LoginForm: React.FC = () => {
  const { login } = useAuth();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
  });

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleLogin = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      setIsLoading(true);
      await login(formData);

      toast.success("Successfully logged in!");
      window.location.href = "/";
    } catch (error) {
      setIsLoading(false);
      console.error("Login failed:", error);
      toast.error("Something went wrong!");
    }
    setIsLoading(false);
  };

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col items-center gap-4">
        <h1 className="">Log ind</h1>
      </div>
      <form className="space-y-2" onSubmit={handleLogin}>
        <div className="flex flex-col gap-2">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            className="py-1 px-3 bg-slate-100 "
          />
        </div>
        <div className="flex flex-col gap-2 pb-5">
          <label htmlFor="password">Kodeord</label>
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            className="py-1 px-3 bg-slate-100 "
          />
        </div>
        {isLoading ? (
          <button className=" border border-orange-300 py-3 px-4 flex justify-center font-medium w-full opacity-50">
            <Loader2 className="animate-spin h-4 w-4" />
          </button>
        ) : (
          <button
            disabled={isLoading}
            type="submit"
            className=" border border-orange-300 py-2 px-4 font-medium w-full disabled:opacity-50"
          >
            Log ind
          </button>
        )}
      </form>
    </div>
  );
};

export default LoginForm;
