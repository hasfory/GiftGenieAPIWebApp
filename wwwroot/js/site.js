const apiUsers = 'api/Users';
const apiWL = 'api/Wishlists';

let currentUser = null;
let wishlists = [];

// Utility to hide all sidebar forms
function hideForms() {
    document.getElementById('regForm').classList.add('hidden');
    document.getElementById('loginForm').classList.add('hidden');
}

// Show register form
function showRegister() {
    hideForms();
    document.getElementById('regForm').classList.remove('hidden');
}
// Show login form
function showLogin() {
    hideForms();
    document.getElementById('loginForm').classList.remove('hidden');
}

// After login/show wishlists
function showWishlists() {
    if (!currentUser) { showLogin(); return; }
    hideForms();
    loadWishlistsView();
}

// ===== AUTH =====

function registerUser() {
    const username = document.getElementById('reg-username').value.trim();
    const password = document.getElementById('reg-password').value;
    const fullName = document.getElementById('reg-fullName').value.trim();
    const birthdate = document.getElementById('reg-birthdate').value;

    fetch(apiUsers, {
        method: 'POST',
        headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password, fullName, birthdate })
    })
        .then(res => {
            if (!res.ok) throw new Error('Registration failed');
            return res.json();
        })
        .then(user => {
            document.getElementById('reg-msg').innerText = 'Registered! Please login.';
        })
        .catch(err => {
            document.getElementById('reg-msg').innerText = err.message;
        });
}

function loginUser() {
    const username = document.getElementById('login-username').value.trim();
    const password = document.getElementById('login-password').value;

    fetch(apiUsers)
        .then(res => res.json())
        .then(users => {
            const user = users.find(u => u.username === username && u.password === password);
            if (!user) throw new Error('Invalid credentials');
            currentUser = user;
            onLoginSuccess();
        })
        .catch(err => {
            document.getElementById('login-msg').innerText = err.message;
        });
}

function onLoginSuccess() {
    document.getElementById('sidebar').classList.add('hidden');
    document.getElementById('profileName').classList.remove('hidden');
    document.getElementById('profileUser').innerText = currentUser.fullName;
    loadWishlistsView();
}

// ===== WISHLISTS VIEW =====

function loadWishlistsView() {
    const main = document.getElementById('mainContent');
    main.innerHTML = `
    <h2>Your Wishlists</h2>
    <p>Total: <span id="counter">0</span></p>
    <form onsubmit="addWishlist(); return false;">
      <input type="text" id="add-title" placeholder="Wishlist Title" required />
      <label><input type="checkbox" id="add-isPublic" /> Public</label>
      <button class="btn">Add Wishlist</button>
    </form>
    <table>
      <thead><tr><th>Title</th><th>Public</th><th colspan="2">Actions</th></tr></thead>
      <tbody id="wishlists"></tbody>
    </table>
    <div id="editForm" class="hidden">
      <h3>Edit Wishlist</h3>
      <form onsubmit="updateWishlist(); return false;">
        <input type="hidden" id="edit-id" />
        <input type="text" id="edit-title" required />
        <label><input type="checkbox" id="edit-isPublic" /> Public</label>
        <button class="btn">Save</button>
        <button class="btn" type="button" onclick="closeEdit()">Cancel</button>
      </form>
    </div>
  `;
    getWishlists();
}

// Fetch and render
function getWishlists() {
    fetch(`${apiWL}/user/${currentUser.userId}`)
        .then(res => res.json())
        .then(_displayWishlists)
        .catch(err => console.error(err));
}
function addWishlist() {
    const title = document.getElementById('add-title').value.trim();
    const isPublic = document.getElementById('add-isPublic').checked;
    fetch(apiWL, {
        method: 'POST', headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId: currentUser.userId, title, isPublic })
    })
        .then(getWishlists)
        .catch(console.error);
}
function deleteWishlist(id) {
    fetch(`${apiWL}/${id}`, { method: 'DELETE' })
        .then(getWishlists)
        .catch(console.error);
}
function displayEditForm(id) {
    const w = wishlists.find(x => x.wishlistId === id);
    document.getElementById('edit-id').value = w.wishlistId;
    document.getElementById('edit-title').value = w.title;
    document.getElementById('edit-isPublic').checked = w.isPublic;
    document.getElementById('editForm').classList.remove('hidden');
}
function updateWishlist() {
    const id = +document.getElementById('edit-id').value;
    const title = document.getElementById('edit-title').value.trim();
    const isPublic = document.getElementById('edit-isPublic').checked;
    fetch(`${apiWL}/${id}`, {
        method: 'PUT', headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        body: JSON.stringify({ wishlistId: id, userId: currentUser.userId, title, isPublic })
    })
        .then(getWishlists)
        .catch(console.error)
        .finally(closeEdit);
}
function closeEdit() {
    document.getElementById('editForm').classList.add('hidden');
}

// Render
function _displayWishlists(data) {
    const tBody = document.getElementById('wishlists');
    tBody.innerHTML = '';
    document.getElementById('counter').innerText = data.length;
    data.forEach(w => {
        const tr = tBody.insertRow();
        tr.insertCell(0).innerText = w.title;
        tr.insertCell(1).innerText = w.isPublic ? 'Yes' : 'No';

        const eCell = tr.insertCell(2), dCell = tr.insertCell(3);
        const btnE = document.createElement('button');
        btnE.innerText = 'Edit'; btnE.onclick = () => displayEditForm(w.wishlistId);
        const btnD = document.createElement('button');
        btnD.innerText = 'Delete'; btnD.onclick = () => deleteWishlist(w.wishlistId);
        eCell.appendChild(btnE); dCell.appendChild(btnD);
    });
    wishlists = data;
}
