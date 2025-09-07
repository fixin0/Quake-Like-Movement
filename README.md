# 🎮 Unity 3D Rigidbody Player Controller

Welcome! This is a **simple 3D player controller** for Unity.
It uses **Rigidbody** physics and allows movement on **ground and air**.

---

## 📦 Features

* ✅ Walking on ground
* ✅ Jumping
* ✅ Air control (change direction while jumping)
* ✅ Friction system for smooth stop
* ✅ Works with Unity Input System

---

## 🛠 Requirements

* Unity **2021.3** or higher
* **Rigidbody** attached to your player
* Unity **Input System Package** installed

---

## 🚀 Setup

1. Clone this repository:

```bash
git clone https://github.com/username/UnityRigidbodyPlayerController.git
```

2. Open Unity and import the folder.
3. Create a **Player GameObject**:

   * Add **Rigidbody** component

     * Interpolation: Interpolate
     * Collision Detection: Continuous Dynamic
   * Add an empty child object and assign it as **groundChecker**
4. Add **PlayerController.cs** to the Player object.
5. Create an **Input Action Asset**:

   * Movement: Vector2
   * Jump: Button
   * Connect it to `Generalnput` class

---

## 🎮 Controls

| Action      | Key / Input                 |
| ----------- | --------------------------- |
| Walk        | WASD / Arrows               |
| Jump        | Space                       |
| Air Control | Move direction while in air |

---

## ⚙ Player Settings

* `moveSpeed` → Walking speed
* `jumpForce` → Jump height
* `groundAccelerate`, `airAccelerate` → Acceleration
* `maxGroundSpeed`, `maxAirSpeed` → Max speed
* `stopSpeed`, `groundFriction` → Friction when stopping

---

## 🧩 Code Structure

* **Update()** → Handle input and move direction
* **FixedUpdate()** → Apply movement with Rigidbody
* **GroundMove()** → Ground movement and speed limit
* **AirMove()** → Air movement and air control
* **HandleJump()** → Jump system
* **Accelerate(), AirAccelerate(), AirControl()** → Velocity control
* **ApplyFriction()** → Apply friction on ground
* **IsGrounded()** → Check if player is on ground

---

## 🔧 Customization

* Change air control with `airControl`
* Adjust max speed with `maxGroundSpeed` and `maxAirSpeed`
* Adjust jump height with `jumpForce`

---

## 🤝 Contributing

1. Fork the project
2. Create a new branch:

```bash
git checkout -b feature-name
```

3. Make changes, commit, and push
4. Open a Pull Request

---
