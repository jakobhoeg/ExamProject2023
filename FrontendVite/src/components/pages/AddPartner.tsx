import { ChangeEvent, FormEvent, useState } from "react";
import { toast } from "sonner";

interface AddPartnerFormData {
  email: string;
}

export default function AddPartner() {
  const [formData, setFormData] = useState<AddPartnerFormData>({
    email: "",
  });

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleLogin = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const response = await fetch("http://localhost:5000/add-partner", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        toast.success("Successfully added partner!");
      } else {
        throw new Error("You can't add this partner");
      }
    } catch (error) {
      console.error("Login failed:", error);
      toast.error("Something went wrong!");
    }
  };

  return (
    <div className="flex flex-col gap-4 w-96 border border-orange-300 p-8 shadow-lg">
      <h1>Tilknyt partner</h1>
      <form onSubmit={handleLogin} className="space-y-4" >
        <input
          type="email"
          name="email"
          placeholder="Email på din partner"
          value={formData.email}
          onChange={handleChange}
          className="inputs w-full p-2"
        />
        <button type="submit" className="border-button w-full">
          Tilknyt
        </button>
      </form>
    </div>
  );
}
