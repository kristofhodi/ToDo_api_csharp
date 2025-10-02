import { useEffect, useState } from "react";
import "./App.css";
import { Link } from "react-router-dom";
import type { Todo } from "./types/todo.ts";
import apiClient from "./apiClient";

function ToDoPage() {
  const [todos, setTodos] = useState<Todo[]>([]);

  useEffect(() => {
    apiClient
      .get("/list")
      .then((response) => {
        console.log("API data:", response.data);
        setTodos(response.data);
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
            {todos.map((todo) => (
              <tr key={todo.id}>
                <td>{todo.id}</td>
                <td>{todo.description}</td>
                <td
                  className={
                    new Date(todo.deadline) < new Date() ? "isRed" : ""
                  }
                >
                  {new Date(todo.deadline).toLocaleDateString()}
                </td>{" "}
                <td>{todo.isReady ? "Ready" : "Not Ready"}</td>
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
