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

  const handleAddPartner = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      const response = await fetch("http://51.20.73.95:5000/api/partner", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        toast.success("Din parter blev tilføjet!");
      } else {
        throw new Error("Du kan ikke tilføje denne partner");
      }
    } catch (error) {
      console.error(error);
      toast.error("Der gik noget galt.");
    }
  };

  return (
    <div className="flex flex-col gap-4 w-96 border border-orange-300 p-8 shadow-lg">
      <h1>Tilknyt partner</h1>
      <form onSubmit={handleAddPartner} className="space-y-4" >
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
