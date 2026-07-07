import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.layam.validators.*

@Composable
fun Wallet_balancesForm(onSubmit: (Map<String, String>) -> Unit) {
    Column(modifier = Modifier.padding(16.dp)) {
        Text("Wallet_balancesForm", style = MaterialTheme.typography.headlineSmall, modifier = Modifier.padding(bottom = 24.dp))
        
        var wallet_idValue by remember { mutableStateOf("") }
        val iswallet_idValid = true
        OutlinedTextField(
            value = wallet_idValue,
            onValueChange = { wallet_idValue = it },
            label = { Text("wallet_id") },
            isError = !iswallet_idValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!iswallet_idValid) {
            Text("Invalid wallet_id", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var available_balanceValue by remember { mutableStateOf("") }
        val isavailable_balanceValid = validate_wallet_balances_available_balance(available_balanceValue)
        OutlinedTextField(
            value = available_balanceValue,
            onValueChange = { available_balanceValue = it },
            label = { Text("available_balance") },
            isError = !isavailable_balanceValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Number),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isavailable_balanceValid) {
            Text("Invalid available_balance", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var locked_balanceValue by remember { mutableStateOf("") }
        val islocked_balanceValid = validate_wallet_balances_locked_balance(locked_balanceValue)
        OutlinedTextField(
            value = locked_balanceValue,
            onValueChange = { locked_balanceValue = it },
            label = { Text("locked_balance") },
            isError = !islocked_balanceValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Number),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!islocked_balanceValid) {
            Text("Invalid locked_balance", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var updated_atValue by remember { mutableStateOf("") }
        val isupdated_atValid = true
        OutlinedTextField(
            value = updated_atValue,
            onValueChange = { updated_atValue = it },
            label = { Text("updated_at") },
            isError = !isupdated_atValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isupdated_atValid) {
            Text("Invalid updated_at", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        Button(
            onClick = { onSubmit(emptyMap()) /* TODO: map state to data class */ },
            modifier = Modifier.fillMaxWidth().padding(top = 16.dp)
        ) {
            Text("Submit")
        }
    }
}
