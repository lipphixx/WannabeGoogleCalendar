<script setup>
import { ref } from 'vue';
import axios from 'axios';
import PasswordReset from './passwordReset.vue';

const email = ref(null);
const password = ref(null);
const emit = defineEmits(['fetchLogin']);
const correctAuth = ref(true);
const showPasswordReset = ref(false);

async function fetchLogin() {
  const url = "https://172.20.10.4:5050/api/Auth/login";
  try {
    const loginDetails = {
      email: email.value,
      password: password.value,
    };
    const response = await axios.post(url, loginDetails);

    console.log('Server Response:', response.data);

    const { token, userId, username } = response.data;

    if (token && userId && username) {
      sessionStorage.setItem("token", token);
      sessionStorage.setItem("loggedUser", JSON.stringify(userId));
      sessionStorage.setItem("loggedUsername", username);
      emit("fetchLogin", true, response.data);
      correctAuth.value = true;
    } else {
      console.warn("Server response is missing expected fields.");
      correctAuth.value = false;
      emit("fetchLogin", false);
    }
  } catch (error) {
    console.error("Error fetching login:", error);
    correctAuth.value = false;
    emit("fetchLogin", false);
  }
}
</script>

<template>
  <div v-if="showPasswordReset" style="margin-bottom: 1%">
    <PasswordReset @close="showPasswordReset = false" />
  </div>
  <div v-else>
    <form @submit.prevent="fetchLogin">
      <h2>Login</h2>
      <div id="inputContainer">
        <label> Email:
          <input class="inputStyle" type="text" placeholder="Email" v-model="email" required>
        </label>
        <label> Heslo:
          <input class="inputStyle" type="password" placeholder="Heslo" v-model="password" required>
        </label>
        <p v-if="!correctAuth" style="color: red">Špatný e-mail nebo heslo.</p>
        <a @click="showPasswordReset = true">Zapomenuté heslo</a>
      </div>
      <button type="submit">Přihlásit se</button>
    </form>
  </div>
</template>

<style scoped>
a {
  cursor: pointer;
  font-size: 90%;
  color: cornflowerblue;
}

a:hover {
  text-decoration: underline;
}

form {
  display: flex;
  width: 100%;
  flex-direction: column;
  align-items: center;
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
  height: 15%;
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
