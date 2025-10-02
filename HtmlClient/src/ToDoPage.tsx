import { useState } from 'react'
import './App.css'
import { Link } from 'react-router-dom'

function ToDoPage() {

  return (
    <>
    <div>ez egy default page</div>
    <Link to="/"><button>go back to main page</button></Link>
    </>
  )
}

export default ToDoPage
