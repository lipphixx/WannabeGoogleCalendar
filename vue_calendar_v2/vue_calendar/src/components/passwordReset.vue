<script setup>
import { ref } from 'vue';
import axios from 'axios';

const recEmail = ref(null);
const recCode = ref(null);
const newPass = ref(null);

const emailCorrect = ref(true);
const codeCorrect = ref(true);

const onEmail = ref(true);
const onCode = ref(false);
const onPassword = ref(false);

const recoveryDetails = ref(null);

const emit = defineEmits(["close"]);

async function resetPassword() {
  const response = await axios.get('https://172.20.10.4:5050/api/Users');
  if (!(response.data.find(x => x.email === recEmail.value))) {
    emailCorrect.value = false;
    return;
  }

  onEmail.value = false;
  emailCorrect.value = true;
  onCode.value = true;
  const array = new Uint32Array(1);
  window.crypto.getRandomValues(array);
  const code = (array[0] % 10000).toString().padStart(4, "0");

  recoveryDetails.value = {
    recoveryEmail: String(recEmail.value),
    recoveryCode: String(code)
  };

  const emailDetails = {
    emailToId: recEmail.value,
    emailToName: '',
    emailSubject: `Kód k obnově hesla`,
    emailBody: `Váš kód k obnovení hesla je ${code}.<br>Heslo nikomu nesdělujte!<br>`
  };

  try {
    await axios.post('https://0.0.0.0:5050/api/Auth/CreateNewRecovery', recoveryDetails.value);
    await axios.post('https://172.20.10.4:5050/api/Mail', emailDetails);
  } catch (error) {
    console.error(`Chyba při odesílání e-mailu:`, error);
  }
}
async function checkRecoveryCode() {
  try {
    const details = {
      recoveryEmail: recoveryDetails.value.recoveryEmail,
      recoveryCode: recCode.value
    };

    const checkResult = await axios.post('https://172.20.10.4:5050/api/auth/CheckCode', details);
    if (checkResult.status !== 200) {
      codeCorrect.value = false;
      return;
    }
    onCode.value = false;
    onPassword.value = true;
  } catch (error) {
    console.log(error);
    codeCorrect.value = false;
  }
}
async function changePassword() {
  try {
    await axios.post('https://0.0.0.0:5050/api/Auth/ChangePassword', {
      email: recEmail.value,
      newPassword: newPass.value
    });
    alert('Změna hesla proběhla úspěšně!');
    emit("close");
  } catch (error) {
    console.log(error);
  }
}
</script>

<template>
    <form v-if="onEmail || onCode || onPassword" @submit.prevent="resetPassword">
      <h2>Obnova hesla</h2>
      <label>
        Email:
        <input class="inputStyle" type="email" placeholder="Email" v-model="recEmail" :disabled="onCode">
      </label>
      <p v-if="!emailCorrect">Email neexistuje!</p>
      <button type="submit" v-if="onEmail">Odeslat</button>
    </form>

    <form v-if="onCode || onPassword" @submit.prevent="checkRecoveryCode">
      <label>
        Ověřovací kód:
        <input class="inputStyle" type="text" placeholder="Kód" v-model="recCode" :disabled="onPassword">
      </label>
      <p v-if="!codeCorrect">Nesprávný kód!</p>
      <button type="submit" v-if="onCode">Odeslat</button>
    </form>

    <form v-if="onPassword" @submit.prevent="changePassword">
      <label>
        Nové heslo:
        <input class="inputStyle" type="password" placeholder="Heslo" v-model="newPass">
      </label>
      <button type="submit">Odeslat</button>
    </form>
</template>

<style scoped>
.reset-container {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 5px;
  background: white;
  max-width: 400px;
}
.inputStyle {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}
button {
  width: 100%;
  padding: 10px;
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 2%;
}
button:hover {
  background-position: right;
}

form {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 20vw;
}

label {
  width: 20vw;
}
</style>
