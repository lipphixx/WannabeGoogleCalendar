<script setup>
import {ref} from "vue";
import axios from "axios";

const email = ref(null);
const password = ref(null);

const emit = defineEmits(["fetchLogin"]);
const props = defineProps(['userId']);

async function fetchLogin() {
  const url = "https://localhost:7198/api/Auth/login";

  try {
    const loginDetails = {
      email: email.value,
      password: password.value,
      twoFactorCode: "1234",
      twoFactorRecoveryCode: "abcd",
    };

    const response = await axios.post(url, loginDetails);
    console.log(response);
    emit("fetchLogin", true, response.data); // Odeslat event do rodiče
    console.log(response.data);
  } catch (error) {
    console.log(error);
    emit("fetchLogin", false);
  }
}
</script>

<template>
  <form @submit.prevent="fetchLogin">
    <h2>Login</h2>
    <div id="inputContainer">
    <label>
      Email:
      <input class="inputStyle" type="text" placeholder="Email" v-model="email">
    </label>
    <label>
      Heslo:
      <input class="inputStyle" type="password" placeholder="Heslo" v-model="password">
    </label>
    </div>
    <button type="submit">Přihlásit se</button>
  </form>
</template>

<style scoped>

form {
  width: 30%;
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