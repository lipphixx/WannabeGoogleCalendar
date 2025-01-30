<script setup>

import {ref} from "vue";
import axios from "axios";

const email = ref(null);
const password = ref(null);

async function fetchLogin(){
  const url = "https://localhost:7198/api/Auth/login";

  try {
    const loginDetails = {
      "email": email.value,
      "password": password.value
    }
    await axios.post(url, loginDetails);
  } catch (error) {
    console.log(error);
  }
}

</script>

<template>
  <form @submit.prevent="fetchLogin()">
    <h2>Login</h2>
    <label>
      Email:
      <input type="text" placeholder="Jméno" v-model="email">
    </label>
    <label>
      Heslo:
      <input type="password" placeholder="Příjmení" v-model="password">
    </label>
    <button type="submit">Přihlásit se</button>
  </form>
</template>

<style scoped>
.login {
  width: 300px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

form div {
  margin-bottom: 10px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  width: 100%;
  padding: 10px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

.error {
  color: red;
  font-size: 14px;
  margin-top: 10px;
}
</style>