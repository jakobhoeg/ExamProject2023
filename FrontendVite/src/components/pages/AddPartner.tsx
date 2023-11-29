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
            await fetch("http://localhost:5000/add-partner", {
                method: "POST",
                credentials: "include",
                headers: {
                "Content-Type": "application/json",
                },
                body: JSON.stringify(formData),
            });
    
          toast.success("Partner added!");
        } catch (error) {
          console.error("Login failed:", error);
          toast.error("Something went wrong!");
        }
      };

  return (
    <div>
        <h1>Tilknyt partner</h1>
        <form onSubmit={handleLogin}>
            <input type="email" name="email" value={formData.email} onChange={handleChange}
            className="inputs w-full" />
            <button type="submit" className="border-button">Tilknyt</button>
        </form>
    </div>
  )
}
