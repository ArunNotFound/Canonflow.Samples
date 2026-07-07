import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.layam.validators.*

@Composable
fun Ledger_transactionsForm(onSubmit: (Map<String, String>) -> Unit) {
    Column(modifier = Modifier.padding(16.dp)) {
        Text("Ledger_transactionsForm", style = MaterialTheme.typography.headlineSmall, modifier = Modifier.padding(bottom = 24.dp))
        
        var transaction_idValue by remember { mutableStateOf("") }
        val istransaction_idValid = true
        OutlinedTextField(
            value = transaction_idValue,
            onValueChange = { transaction_idValue = it },
            label = { Text("transaction_id") },
            isError = !istransaction_idValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!istransaction_idValid) {
            Text("Invalid transaction_id", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

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

        var amountValue by remember { mutableStateOf("") }
        val isamountValid = validate_ledger_transactions_amount(amountValue)
        OutlinedTextField(
            value = amountValue,
            onValueChange = { amountValue = it },
            label = { Text("amount") },
            isError = !isamountValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Number),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isamountValid) {
            Text("Invalid amount", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var currencyValue by remember { mutableStateOf("") }
        val iscurrencyValid = true
        OutlinedTextField(
            value = currencyValue,
            onValueChange = { currencyValue = it },
            label = { Text("currency") },
            isError = !iscurrencyValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!iscurrencyValid) {
            Text("Invalid currency", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var directionValue by remember { mutableStateOf("") }
        val isdirectionValid = validate_ledger_transactions_direction(directionValue)
        OutlinedTextField(
            value = directionValue,
            onValueChange = { directionValue = it },
            label = { Text("direction") },
            isError = !isdirectionValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isdirectionValid) {
            Text("Invalid direction", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var statusValue by remember { mutableStateOf("") }
        val isstatusValid = validate_ledger_transactions_status(statusValue)
        OutlinedTextField(
            value = statusValue,
            onValueChange = { statusValue = it },
            label = { Text("status") },
            isError = !isstatusValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isstatusValid) {
            Text("Invalid status", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var reference_idValue by remember { mutableStateOf("") }
        val isreference_idValid = true
        OutlinedTextField(
            value = reference_idValue,
            onValueChange = { reference_idValue = it },
            label = { Text("reference_id") },
            isError = !isreference_idValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!isreference_idValid) {
            Text("Invalid reference_id", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        var created_atValue by remember { mutableStateOf("") }
        val iscreated_atValid = true
        OutlinedTextField(
            value = created_atValue,
            onValueChange = { created_atValue = it },
            label = { Text("created_at") },
            isError = !iscreated_atValid,
            keyboardOptions = androidx.compose.foundation.text.KeyboardOptions(keyboardType = androidx.compose.ui.text.input.KeyboardType.Text),
            modifier = Modifier.fillMaxWidth().padding(bottom = 16.dp)
        )
        if (!iscreated_atValid) {
            Text("Invalid created_at", color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        Button(
            onClick = { onSubmit(emptyMap()) /* TODO: map state to data class */ },
            modifier = Modifier.fillMaxWidth().padding(top = 16.dp)
        ) {
            Text("Submit")
        }
    }
}
