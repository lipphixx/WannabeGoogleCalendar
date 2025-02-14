<script setup>
import {onMounted, ref} from "vue";
import axios from "axios";

const email = ref(null);
const password = ref(null);
const emit = defineEmits(["fetchLogin"]);

async function fetchLogin() {
  const url = "https://localhost:5050/api/Auth/login";

  try {
    const loginDetails = {
      email: email.value,
      password: password.value,
      twoFactorCode: "1234",
      twoFactorRecoveryCode: "abcd",
    };

    const response = await axios.post(url, loginDetails);

    localStorage.setItem("loggedUser", JSON.stringify(response.data.userId));
    localStorage.setItem("loggedUsername", response.data.username);
    emit("fetchLogin", true, response.data);

  } catch (error) {
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
  display: flex;
  width: 100%;
}

label {
  display: flex;
  flex-direction: column;
}

#inputContainer {
  margin-bottom: 10px;
  display: flex;
  flex-direction: column;
}

input {
  width: 300px;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  flex-shrink: 0;
  height: 10%;
  width: 300px;
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  color: White;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-position: right;
}

</style>