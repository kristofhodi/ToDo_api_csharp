import { useEffect, useState } from "react";
import "./App.css";
import { Link } from "react-router-dom";
import type { Todo } from "./types/todo.ts";
import apiClient from "./apiClient";

function ToDoPage() {
  const [data, setData] = useState<Todo[]>([]);

  useEffect(() => {
    apiClient
      .get("/list")
      .then((response) => {
        console.log("API data:", response.data); // <-- check data structure here
        setData(response.data);
      })
      .catch((error) => {
        if (error.response) {
          console.error("Szerver válasz hiba:", error.response.status);
        } else if (error.request) {
          console.error("Nincs válasz a szervertől:", error.request);
        } else {
          console.error("Axios hiba:", error.message);
        }
      });
  }, []);

  return (
    <>
      <div className="App">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Description</th>
              <th>Deadline</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {data.map((sz) => (
              <tr key={sz.id}> {/* Make sure _id exists */}
                <td>{sz.id}</td>
                <td>{sz.description}</td>
                <td>{new Date(sz.deadline).toLocaleDateString()}</td>
                <td>{sz.isReady ? "Ready" : "Not Ready"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <Link to="/">
        <button>go back to main page</button>
      </Link>
    </>
  );
}

export default ToDoPage;
